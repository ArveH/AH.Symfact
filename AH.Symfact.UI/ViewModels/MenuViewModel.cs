using AH.Symfact.UI.Controls;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;

namespace AH.Symfact.UI.ViewModels;

public partial class MenuViewModel : ObservableRecipient
{
    private readonly ILogger _logger;

    public MenuViewModel(ILogger logger)
    {
        _logger = logger.ForContext<MenuControl>();
    }

    [RelayCommand]
    private void LoadConnectDetails()
    {
        _logger.Information("Loading Connect page details");
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.Connect));
    }

    [RelayCommand]
    private void LoadCreateTablesDetails()
    {
        _logger.Information("Loading Create Tables page details");
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.CreateTables));
    }
}