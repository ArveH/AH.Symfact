using AH.Symfact.UI.Models;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AH.Symfact.UI.ViewModels;

public partial class CreateTablesViewModel : ObservableRecipient
{
    private readonly ITableService _tableService;
    private readonly ILogger _logger;

    public CreateTablesViewModel(
        ITableService tableService,
        ILogger logger)
    {
        _tableService = tableService;
        _logger = logger.ForContext<CreateTablesViewModel>();
        SourceTableCommand = new AsyncRelayCommand(CreateWithNoColumns);
        ComputedColumnsCommand = new AsyncRelayCommand(CreateWithComputedColumns);
        ExtractedColumnsCommand = new AsyncRelayCommand(CreateWithExtractedColumns);
        WeakReferenceMessenger.Default.Register<TableChangedMessage>(this, (_, msg) =>
        {
            if (msg.Value.Action == TableAction.LoadedXml)
            {
                Heading = $"Contract tables {msg.Value.Rows} rows";
                return;
            }
            switch (msg.Value.TableName)
            {
                case "Contract":
                    SourceTableStatus = msg.Value.Message ?? "<Message missing>";
                    break;
                case "ContractComputedCols":
                    ComputedColumnsStatus = msg.Value.Message ?? "<Message missing>";
                    break;
                case "ContractExtractedCols":
                    ComputedColumnsStatus = msg.Value.Message ?? "<Message missing>";
                    break;
            }
        });
    }

    [ObservableProperty]
    private string _heading = "Contract tables";
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
        await _tableService.CreateTableAsync("Contract", "Contract.sql", "Contract.xml");
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