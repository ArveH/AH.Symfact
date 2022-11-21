using AH.Symfact.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Threading.Tasks;

namespace AH.Symfact.UI.ViewModels;

public partial class TablesViewModel : ObservableRecipient
{
    private readonly ILogger _logger;

    public TablesViewModel(ILogger logger)
    {
        _logger = logger.ForContext<MenuControl>();
    }

    [RelayCommand]
    private async Task CreateTableAsync(string tableName)
    {
        _logger.Information("Creating table '{TableName}'", tableName);
        await Task.CompletedTask;
    }
}