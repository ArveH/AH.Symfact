using AH.Symfact.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AH.Symfact.UI.ViewModels;

public partial class TablesViewModel : ObservableRecipient
{
    private readonly ILogger _logger;

    public TablesViewModel(ILogger logger)
    {
        _logger = logger.ForContext<MenuControl>();
        DeleteAllTablesCommand = new AsyncRelayCommand(DeleteAllTablesAsync);
        CreateAllTablesCommand = new AsyncRelayCommand(CreateAllTablesAsync);
    }

    [ObservableProperty]
    private string _deleteAllStatus = "Ready...";
    [ObservableProperty]
    private string _createAllStatus = "Ready...";

    public ICommand DeleteAllTablesCommand { get; }
    public ICommand CreateAllTablesCommand { get; }

    private async Task DeleteAllTablesAsync()
    {
        _logger.Information("Deleting all tables...");
        DeleteAllStatus = "Deleting all tables...";


        await Task.CompletedTask;
    }

    private async Task CreateAllTablesAsync()
    {
        _logger.Information("Creating all tables...");
        CreateAllStatus = "Creating all tables...";


        await Task.CompletedTask;
    }
}