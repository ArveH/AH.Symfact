using AH.Symfact.UI.Controls;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Models;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        DeleteAllTablesCommand = new AsyncRelayCommand(DeleteAllTablesAsync);
        SelectDataFolderCommand = new AsyncRelayCommand(SelectDataFolderAsync);
        CreateAllTablesCommand = new AsyncRelayCommand(CreateAllTablesAsync);
    }

    [ObservableProperty]
    private string _deleteAllStatus = "Ready...";
    [ObservableProperty]
    private string _dataPath = @"D:\Temp\Symfact\DataSet";
    [ObservableProperty]
    private string _createAllStatus = "Ready...";

    public ICommand DeleteAllTablesCommand { get; }
    public ICommand SelectDataFolderCommand { get; }
    public ICommand CreateAllTablesCommand { get; }

    private async Task DeleteAllTablesAsync()
    {
        try
        {
            _logger.Information("Deleting all tables...");
            DeleteAllStatus = "Deleting all tables...";

            var tables = await _dbCommands.GetAllTablesAsync();
            if (tables.Count < 1)
            {
                _logger.Information("No tables deleted");
                DeleteAllStatus = "No tables deleted";
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"Do you want to delete the following {tables.Count} tables?");
            tables.ForEach(t => sb.AppendLine("\t" + t));
            var deleteTablesDialog = new ContentDialog
            {
                Title = "Delete Tables",
                Content = sb.ToString(),
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };
            var xamlRoot = WeakReferenceMessenger.Default.Send<XamlRootMessage>();
            deleteTablesDialog.XamlRoot = xamlRoot;
            var result = await deleteTablesDialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                _logger.Information("No tables deleted");
                DeleteAllStatus = "No tables deleted";
                return;
            }
            await _dbCommands.DeleteTablesAsync(tables);

            _logger.Information("{TableCount} tables deleted", tables.Count);
            DeleteAllStatus = $"{tables.Count} tables deleted";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error when deleting tables");
            DeleteAllStatus = ex.FlattenMessages();
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
        var entityName = "Contract";
        var contractData = ReadFromXml(entityName);
        try
        {
            _logger.Information("Creating table '{TableName}'...",
                entityName);
            CreateAllStatus = $"Creating table '{entityName}'...";
            var folder = WeakReferenceMessenger.Default.Send<ExeFolderMessage>();
            var filePath = Path.Combine(folder, "Database", "Scripts", "CreateContractTable.sql");
            var cmds = await File.ReadAllTextAsync(filePath);
            await _dbCommands.ExecuteScriptAsync(cmds);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't create table {TableName}", entityName);
            CreateAllStatus = $"Can't create table '{entityName}'...";
        }
    }

    private IEnumerable<TableRow>? ReadFromXml(string entityName)
    {
        var filePath = Path.Combine(DataPath, $"{entityName}.xml");
        _logger.Information("Starting to load from file '{FilePath}'...",
            filePath);
        CreateAllStatus = $"Starting to load from file '{filePath}'...";
        try
        {
            var xElem = XElement.Load(filePath);
            var elemName = _fileReader.GetName(xElem);
            if (elemName == null)
            {
                _logger.Error("Can't find element name for {EntityName}", entityName);
                CreateAllStatus = $"Can't find element name for {entityName}";
                return null;
            }

            var xmlData = _fileReader.SplitRequests(xElem);
            WeakReferenceMessenger.Default.Send(new XmlFileLoadedMessage(new XmlFileInfo
            {
                ContractElementName = elemName,
                ContractCount = xmlData.Count
            }));
            _logger.Information("Found {RowCount} rows for '{ElementName}'",
                xmlData.Count, elemName);
            CreateAllStatus = $"Found {xmlData.Count} rows for '{elemName}'";
            return xmlData;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't read XML file for {EntityName}", entityName);
            CreateAllStatus = $"Can't read XML file for {entityName}";
            return null;
        }
    }
}