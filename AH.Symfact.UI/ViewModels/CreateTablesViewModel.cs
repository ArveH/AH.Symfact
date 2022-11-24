using AH.Symfact.UI.Models;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AH.Symfact.UI.ViewModels;

public partial class CreateTablesViewModel : ObservableRecipient
{
    private readonly ITableService _tableService;

    public CreateTablesViewModel(
        ITableService tableService)
    {
        _tableService = tableService;
        SourceTableCommand = new AsyncRelayCommand(CreateWithNoColumns);
        ComputedColumnsCommand = new AsyncRelayCommand(CreateWithComputedColumns);
        ExtractedColumnsCommand = new AsyncRelayCommand(CreateWithExtractedColumns);
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
            else if (msg.Value.TableName == TableName+"ComputedCols")
                ComputedColumnsStatus = msg.Value.Message ?? "<Message missing>";
            else if (msg.Value.TableName == TableName+"ExtractedCols") ComputedColumnsStatus = msg.Value.Message ?? "<Message missing>";
        });
    }

    [ObservableProperty]
    private string _heading = "";
    [ObservableProperty]
    private string _tableName = "";
    partial void OnTableNameChanging(string value) {Heading = $"{value} tables";}
    [ObservableProperty]
    private string _sourceTableStatus = "Ready...";
    [ObservableProperty]
    private string _computedColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _extractedColumnsStatus = "Ready...";

    public ICommand SourceTableCommand { get; }
    public ICommand ComputedColumnsCommand { get; }
    public ICommand ExtractedColumnsCommand { get; }

    private async Task CreateWithNoColumns()
    {
        await _tableService.CreateTableAsync(TableName, $"{TableName}.sql", $"{TableName}.xml");
    }

    private Task CreateWithComputedColumns()
    {
        throw new NotImplementedException();
    }

    private Task CreateWithExtractedColumns()
    {
        throw new NotImplementedException();
    }
}