using System.Threading.Tasks;
using AH.Symfact.UI.Controls;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Windows.Input;

namespace AH.Symfact.UI.ViewModels;

public partial class CreateTablesViewModel : ObservableRecipient
{
    private readonly IDbCommands _dbCommands;
    private readonly ITaminoFileReader _fileReader;
    private readonly ILogger _logger;

    public CreateTablesViewModel(
        IDbCommands dbCommands,
        ITaminoFileReader fileReader,
        ILogger logger)
    {
        _dbCommands = dbCommands;
        _fileReader = fileReader;
        _logger = logger.ForContext<MenuControl>();
        NoColumnsCommand = new AsyncRelayCommand(CreateWithNoColumns);
        ComputedColumnsCommand = new AsyncRelayCommand(CreateWithComputedColumns);
        ExtractedColumnsCommand = new AsyncRelayCommand(CreateWithExtractedColumns);
    }

    [ObservableProperty]
    private string _heading = "Contract tables";
    [ObservableProperty]
    private string _noColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _computedColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _extractedColumnsStatus = "Ready...";

    public ICommand NoColumnsCommand { get; }
    public ICommand ComputedColumnsCommand { get; }
    public ICommand ExtractedColumnsCommand { get; }

    private Task CreateWithNoColumns()
    {
        throw new System.NotImplementedException();
    }

    private Task CreateWithComputedColumns()
    {
        throw new System.NotImplementedException();
    }

    private Task CreateWithExtractedColumns()
    {
        throw new System.NotImplementedException();
    }
}