using System;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;

namespace AH.Symfact.UI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private IntPtr _hWnd;

    [ObservableProperty]
    private bool _isConnectPage = true;
    [ObservableProperty]
    private bool _isCreateTablesPage;

    public MainViewModel(ILogger logger)
    {
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            switch (msg.Value)
            {
                case PageName.Connect:
                    IsConnectPage = true;
                    IsCreateTablesPage = false;
                    break;
                case PageName.CreateTables:
                    IsConnectPage = false;
                    IsCreateTablesPage = true;
                    break;
                default:
                    logger.Error("Illegal value for PageName enum {PageName}", msg.Value);
                    break;
            }
        });

        WeakReferenceMessenger.Default.Register<MainViewModel, WindowHandleSetMessage>(this, (r, m) =>
        {
            m.Reply(r.HWnd);
        });
    }
}