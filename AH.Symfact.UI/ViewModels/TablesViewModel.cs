using AH.Symfact.UI.Controls;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Models;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace AH.Symfact.UI.ViewModels;

public partial class TablesViewModel : ObservableRecipient
{
    private readonly IDbCommands _dbCommands;
    private readonly ITaminoFileReader _fileReader;
    private readonly ILogger _logger;

    public TablesViewModel(
        IDbCommands dbCommands,
        ITaminoFileReader fileReader,
        ILogger logger)
    {
        _dbCommands = dbCommands;
        _fileReader = fileReader;
        _logger = logger.ForContext<MenuControl>();
        CreateSchemasCommand = new AsyncRelayCommand(CreateSchemasAsync);
        SelectDataFolderCommand = new AsyncRelayCommand(SelectDataFolderAsync);
        CreateTablesCommand = new AsyncRelayCommand(CreateAllTablesAsync);
        WeakReferenceMessenger.Default.Register<TablesViewModel, DataFolderChangedMessage>(this, (r, m) =>
        {
            m.Reply(r.DataPath);
        });
    }

    [ObservableProperty]
    private string _createSchemasStatus = "Ready...";
    [ObservableProperty]
    private string _dataPath = @"D:\Temp\Symfact\DataSet";
    [ObservableProperty]
    private string _createTablesStatus = "Ready...";

    public ICommand CreateSchemasCommand { get; }
    public ICommand SelectDataFolderCommand { get; }
    public ICommand CreateTablesCommand { get; }

    private async Task CreateSchemasAsync()
    {
        var createSchemas = new ContentDialog
        {
            Title = "Create Schema Collections",
            Content = "This will delete all existing Schema collections and ALL TABLES!",
            PrimaryButtonText = "Continue",
            CloseButtonText = "Cancel"
        };
        var xamlRoot = WeakReferenceMessenger.Default.Send<XamlRootMessage>();
        createSchemas.XamlRoot = xamlRoot;
        var result = await createSchemas.ShowAsync();
        if (result != ContentDialogResult.Primary)
        {
            _logger.Debug("Create schemas cancelled");
            return;
        }

        await DeleteTablesAsync();

        await ExecuteScriptAsync("SchemaContractXCol.sql", s => CreateSchemasStatus = s);
        await ExecuteScriptAsync("SchemaContractXOrg.sql", s => CreateSchemasStatus = s);
    }

    private async Task DeleteTablesAsync()
    {
        try
        {
            _logger.Information("Deleting all tables...");
            CreateSchemasStatus = "Deleting all tables...";

            var tables = await _dbCommands.GetAllTablesAsync();
            if (tables.Count < 1)
            {
                _logger.Information("No tables deleted");
                CreateSchemasStatus = "No tables deleted";
                return;
            }

            await _dbCommands.DeleteTablesAsync(tables);

            _logger.Information("{TableCount} tables deleted", tables.Count);
            CreateSchemasStatus = $"{tables.Count} tables deleted";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error when deleting tables");
            CreateSchemasStatus = ex.FlattenMessages();
        }
    }

    private async Task SelectDataFolderAsync()
    {
        try
        {
            var hWnd = WeakReferenceMessenger.Default.Send<WindowHandleMessage>();
            var folderPicker = new FolderPicker();
            InitializeWithWindow.Initialize(folderPicker, hWnd);
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.CommitButtonText = "Select";
            var folder = await folderPicker.PickSingleFolderAsync();
            if (!string.IsNullOrWhiteSpace(folder?.Path))
            {
                DataPath = folder.Path;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to select data folder");
        }
    }

    private async Task CreateAllTablesAsync()
    {
        var tasks = new List<Task>
        {
            CreateTableAsync("Contract",
                cnt => WeakReferenceMessenger.Default.Send(new ContractLoadedMessage(cnt))),
            CreateTableAsync("Party",
                cnt => WeakReferenceMessenger.Default.Send(new PartyLoadedMessage(cnt))),
            CreateTableAsync("OrganisationalPerson",
                cnt => WeakReferenceMessenger.Default.Send(new OrgPersonLoadedMessage(cnt)))
        };
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (AggregateException ex)
        {
            for (var i = 0; i < ex.InnerExceptions.Count; i++)
            {
                _logger.Error(ex, ex.Message);
            }
        }
    }

    private async Task CreateTableAsync(string entityName, Func<int, ValueChangedMessage<int>> func)
    {
        await ExecuteScriptAsync(entityName + ".sql", s => CreateTablesStatus = s);
        var contractData = ReadFromXml(entityName);
        try
        {
            func(0);
            _logger.Information("Inserting into {TableName}...", entityName);
            CreateTablesStatus = $"Inserting into {entityName}...";
            var cnt = await _dbCommands.InsertRowsAsync(entityName, contractData);
            _logger.Information("{RowCount} rows inserted into {TableName}", cnt, entityName);
            CreateTablesStatus = $"{cnt} rows inserted into {entityName}";
            func(cnt);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Insert into {TableName} failed", entityName);
            CreateTablesStatus = $"Insert into {entityName} failed. " + ex.FlattenMessages();
        }
    }

    private async Task ExecuteScriptAsync(
        string fileName, Action<string> writeLog)
    {
        try
        {
            _logger.Information("Running script '{FileName}'...", fileName);
            writeLog($"Running script '{fileName}'...");
            var folder = WeakReferenceMessenger.Default.Send<ExeFolderMessage>();
            var filePath = Path.Combine(folder, "Database", "Scripts", fileName);
            var scriptTxt = await File.ReadAllTextAsync(filePath);
            await _dbCommands.ExecuteScriptAsync(
                scriptTxt.Replace("<<<PATH>>>", DataPath));
            _logger.Information("Script '{FileName}' finished", fileName);
            writeLog($"Script '{fileName}' finished");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed running script '{FileName}'", fileName);
            writeLog($"FAILED running script '{fileName}'! " + ex.FlattenMessages());
        }
    }

    private IEnumerable<TableRow>? ReadFromXml(string entityName)
    {
        var filePath = Path.Combine(DataPath, $"{entityName}.xml");
        _logger.Information("Starting to load from file '{FilePath}'...",
            filePath);
        CreateTablesStatus = $"Starting to load from file '{filePath}'...";
        try
        {
            var xElem = XElement.Load(filePath);
            var elemName = _fileReader.GetName(xElem);
            if (elemName == null)
            {
                _logger.Error("Can't find element name for {EntityName}", entityName);
                CreateTablesStatus = $"Can't find element name for {entityName}";
                return null;
            }

            var xmlData = _fileReader.SplitRequests(xElem);
            _logger.Information("Found {RowCount} rows for '{ElementName}'",
                xmlData.Count, elemName);
            CreateTablesStatus = $"Found {xmlData.Count} rows for '{elemName}'";
            return xmlData;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't read XML file for {EntityName}", entityName);
            CreateTablesStatus = $"Can't read XML file for {entityName}";
            return null;
        }
    }
}