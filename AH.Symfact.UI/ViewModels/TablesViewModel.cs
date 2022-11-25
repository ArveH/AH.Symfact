using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace AH.Symfact.UI.ViewModels;

public partial class TablesViewModel : ObservableRecipient
{
    private readonly ISchemaService _schemaService;
    private readonly ITableService _tableService;
    private readonly IDbCommands _dbCommands;
    private readonly ILogger _logger;

    public TablesViewModel(
        ISchemaService schemaService,
        ITableService tableService,
        IDbCommands dbCommands,
        ILogger logger)
    {
        _schemaService = schemaService;
        _tableService = tableService;
        _dbCommands = dbCommands;
        _logger = logger.ForContext<TablesViewModel>();
        CreateSchemasCommand = new AsyncRelayCommand(CreateSchemasAsync);
        SelectDataFolderCommand = new AsyncRelayCommand(SelectDataFolderAsync);
        CreateTablesCommand = new AsyncRelayCommand(CreateAllTablesAsync);
        CreateTablesCommand.CanExecuteChanged += OnCanExecuteChanged;
        CreateFullTextIndexesCommand = new AsyncRelayCommand(CreateFullTextIndexesAsync);
        WeakReferenceMessenger.Default.Register<TablesViewModel, DataFolderChangedMessage>(this, (r, m) =>
        {
            m.Reply(r.DataPath);
        });
        ContractViewModel = new CreateTablesViewModel(_tableService);
        ContractViewModel.TableName = "Contract";
        PartyViewModel = new CreateTablesViewModel(_tableService);
        PartyViewModel.TableName = "Party";
        OrgPersonViewModel = new CreateTablesViewModel(_tableService);
        OrgPersonViewModel.TableName = "OrganisationalPerson";
    }

    private void OnCanExecuteChanged(object? sender, EventArgs e)
    {
        if (sender is not AsyncRelayCommand cmd) return;
        IsCreateAvailable = !cmd.IsRunning;
    }

    public CreateTablesViewModel ContractViewModel { get; set; }
    public CreateTablesViewModel PartyViewModel { get; set; }
    public CreateTablesViewModel OrgPersonViewModel { get; set; }

    [ObservableProperty]
    private string _createSchemasStatus = "Ready...";
    [ObservableProperty]
    private string _dataPath = @"D:\Temp\Symfact\DataSet";
    [ObservableProperty]
    private bool _isCreateAvailable = true;

    public IAsyncRelayCommand CreateSchemasCommand { get; }
    public IAsyncRelayCommand SelectDataFolderCommand { get; }
    public IAsyncRelayCommand CreateTablesCommand { get; }
    public IAsyncRelayCommand CreateFullTextIndexesCommand { get; }

    private async Task CreateSchemasAsync()
    {
        var createSchemas = new ContentDialog
        {
            Title = "Create Schema Collections",
            Content = "This will delete all existing Schema collections and ALL TABLES AND FUNCTIONS!",
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
        await DeleteFunctionsAsync();
        await DeleteSchemaCollectionsAsync();

        await CreateSchemaCollectionAsync(
            SymfactConstants.ContractXCol, SymfactConstants.ContractXColFiles);
        await CreateSchemaCollectionAsync(
            SymfactConstants.ContractXOrg, SymfactConstants.ContractXOrgFiles);
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

            _logger.Information("{Count} tables deleted", tables.Count);
            CreateSchemasStatus = $"{tables.Count} tables deleted";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error when deleting tables");
            CreateSchemasStatus = ex.FlattenMessages();
        }
    }

    private async Task DeleteFunctionsAsync()
    {
        try
        {
            _logger.Information("Deleting all functions...");
            CreateSchemasStatus = "Deleting all functions...";

            var functions = await _dbCommands.GetAllFunctionsAsync();
            if (functions.Count < 1)
            {
                _logger.Information("No functions deleted");
                CreateSchemasStatus = "No functions deleted";
                return;
            }

            await _dbCommands.DeleteFunctionsAsync(functions);

            _logger.Information("{Count} functions deleted", functions.Count);
            CreateSchemasStatus = $"{functions.Count} functions deleted";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error when deleting functions");
            CreateSchemasStatus = ex.FlattenMessages();
        }
    }

    private async Task DeleteSchemaCollectionsAsync()
    {
        try
        {
            _logger.Information("Deleting all schema collections...");
            CreateSchemasStatus = "Deleting all schema collections...";

            var collections = await _dbCommands.GetAllSchemaCollectionsAsync();
            if (collections.Count < 1)
            {
                _logger.Information("No schema collections deleted");
                CreateSchemasStatus = "No schema collections deleted";
                return;
            }

            await _dbCommands.DeleteSchemaCollectionsAsync(collections);

            _logger.Information("{Count} schema collections deleted", collections.Count);
            CreateSchemasStatus = $"{collections.Count} schema collections deleted";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error when deleting schema collections");
            CreateSchemasStatus = ex.FlattenMessages();
        }
    }

    private async Task CreateSchemaCollectionAsync(
        string schemaCollectionName, IReadOnlyList<string> fileNames)
    {
        var ret = await _schemaService.CreateCollectionAsync(
            schemaCollectionName, fileNames[0]);
        if (ret)
        {
            CreateSchemasStatus =
                $"Schema file '{fileNames[0]}' " +
                $"successfully added to schema collection '{schemaCollectionName}'";
        }
        else
        {
            CreateSchemasStatus =
                $"Adding Schema file '{fileNames[0]}' " +
                $"to schema collection '{schemaCollectionName}' failed!";
            return;
        }

        for (var i = 1; i < fileNames.Count; i++)
        {
            ret = await _schemaService.AddToCollectionAsync(
                schemaCollectionName, fileNames[i]);
            if (ret)
            {
                CreateSchemasStatus =
                    $"Schema file '{fileNames[0]}' " +
                    $"successfully added to schema collection '{schemaCollectionName}'";
            }
            else
            {
                CreateSchemasStatus =
                    $"Adding Schema file '{fileNames[0]}' " +
                    $"to schema collection '{schemaCollectionName}' failed!";
                return;
            }
        }

        _logger.Information("All schemas added to schema collection '{SchemaCollectionName}'",
            schemaCollectionName);
        CreateSchemasStatus = $"All schemas added to schema collection '{schemaCollectionName}'";
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
            _tableService.CreateTableAsync("Contract", "Contract.sql", "Contract.xml"),
            _tableService.CreateTableAsync("Party", "Party.sql", "Party.xml"),
            _tableService.CreateTableAsync("OrganisationalPerson", "OrganisationalPerson.sql", "OrganisationalPerson.xml"),
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

    private Task CreateFullTextIndexesAsync()
    {
        throw new NotImplementedException();
    }
}