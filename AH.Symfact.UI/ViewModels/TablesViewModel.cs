using System.Linq;
using AH.Symfact.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Threading.Tasks;
using System.Windows.Input;
using AH.Symfact.UI.Database;

namespace AH.Symfact.UI.ViewModels;

public partial class TablesViewModel : ObservableRecipient
{
    private readonly IDbCommands _dbCommands;
    private readonly ILogger _logger;

    public TablesViewModel(
        IDbCommands dbCommands,
        ILogger logger)
    {
        _dbCommands = dbCommands;
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

        var tables = (await _dbCommands.GetAllTablesAsync()).ToList();
        if (tables.Count < 1)
        {
            _logger.Information("No tables deleted");
            DeleteAllStatus = "No tables deleted";
            return;
        }

        _logger.Information("{TableCount} tables deleted", tables.Count);
        DeleteAllStatus = $"{tables.Count} tables deleted";
    }

    private async Task CreateAllTablesAsync()
    {
        _logger.Information("Creating all tables...");
        CreateAllStatus = "Creating all tables...";


        await Task.CompletedTask;
    }
}