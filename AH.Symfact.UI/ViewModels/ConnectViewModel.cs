using AH.Symfact.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Threading.Tasks;

namespace AH.Symfact.UI.ViewModels;

public partial class ConnectViewModel : ObservableRecipient
{
    private readonly ILogger _logger;

    public ConnectViewModel(ILogger logger)
    {
        _logger = logger.ForContext<MenuControl>();
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        _logger.Information("Connecting...");
        await Task.CompletedTask;
    }
}