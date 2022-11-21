using AH.Symfact.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Threading.Tasks;

namespace AH.Symfact.UI.ViewModels;

public partial class CreateTablesViewModel : ObservableRecipient
{
    private readonly ILogger _logger;

    public CreateTablesViewModel(ILogger logger)
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