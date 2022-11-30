using AH.Symfact.UI.Models;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace AH.Symfact.UI.ViewModels;

public partial class CreateTablesViewModel : ObservableRecipient
{
    private readonly ITableService _tableService;

    public CreateTablesViewModel(
        ITableService tableService)
    {
        _tableService = tableService;
        SourceTableCommand = new AsyncRelayCommand(CreateWithNoColumns);
        ComputedColumnsCommand = new AsyncRelayCommand(CreateWithComputedColumnsAsync);
        ExtractedColumnsCommand = new AsyncRelayCommand(CreateWithExtractedColumnsAsync);
        SelectiveIndexCommand = new AsyncRelayCommand(CreateWithSelectiveIndexAsync);
        NoSchemaCommand = new AsyncRelayCommand(CreateWithNoSchemaAsync);
        WeakReferenceMessenger.Default.Register<TableChangedMessage>(this, (_, msg) =>
        {
            if (!msg.Value.TableName.StartsWith(TableName)) return;

            if (msg.Value.Action == TableAction.LoadedXml)
            {
                Heading = $"{TableName} tables {msg.Value.Rows} rows";
                return;
            }

            if (msg.Value.TableName == TableName)
                SourceTableStatus = msg.Value.Message ?? "<Message missing>";
            else if (msg.Value.TableName == TableName + "SelectiveIndex")
                SelectiveIndexStatus = msg.Value.Message ?? "<Message missing>";
            else if (msg.Value.TableName == TableName + "ComputedColumns")
                ComputedColumnsStatus = msg.Value.Message ?? "<Message missing>";
            else if (msg.Value.TableName == TableName + "ExtractedColumns")
                ExtractedColumnsStatus = msg.Value.Message ?? "<Message missing>";
            else if (msg.Value.TableName == TableName + "NoSchema")
                NoSchemaColumnsStatus = msg.Value.Message ?? "<Message missing>";
        });
    }

    [ObservableProperty]
    private string _heading = "";
    [ObservableProperty]
    private string _tableName = "";
    partial void OnTableNameChanging(string value) { Heading = $"{value} tables"; }
    [ObservableProperty]
    private string _sourceTableStatus = "Ready...";
    [ObservableProperty]
    private string _selectiveIndexStatus = "Ready...";
    [ObservableProperty]
    private string _computedColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _extractedColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _noSchemaColumnsStatus = "Ready...";

    public IAsyncRelayCommand SourceTableCommand { get; }
    public IAsyncRelayCommand SelectiveIndexCommand { get; }
    public IAsyncRelayCommand ComputedColumnsCommand { get; }
    public IAsyncRelayCommand ExtractedColumnsCommand { get; }
    public IAsyncRelayCommand NoSchemaCommand { get; }

    private Task CreateWithNoColumns()
    {
        return _tableService.CreateTableAsync(TableName, $"{TableName}.sql", $"{TableName}.xml");
    }

    private Task CreateWithNoSchemaAsync()
    {
        var tableName = TableName + "NoSchema";
        return _tableService.ExecuteScriptAsync(tableName, $"{tableName}.sql");
    }

    private Task CreateWithSelectiveIndexAsync()
    {
        var tableName = TableName + "SelectiveIndex";
        return _tableService.ExecuteScriptAsync(tableName, $"{tableName}.sql");
    }

    private Task CreateWithComputedColumnsAsync()
    {
        var tableName = TableName + "ComputedColumns";
        return _tableService.ExecuteScriptAsync(tableName, $"{tableName}.sql");
    }

    private Task CreateWithExtractedColumnsAsync()
    {
        var tableName = TableName + "ExtractedColumns";
        return _tableService.ExecuteScriptAsync(tableName, $"{tableName}.sql");
    }
}