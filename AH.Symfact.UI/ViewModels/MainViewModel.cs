using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Serilog;
using System;

namespace AH.Symfact.UI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private IntPtr _hWnd;
    [ObservableProperty]
    private XamlRoot? _xamlRoot;
    [ObservableProperty]
    private string _exeFolder = "";

    [ObservableProperty]
    private bool _isConnectPage = true;
    [ObservableProperty]
    private bool _isTablesPage;
    [ObservableProperty]
    private bool _isTestingPage;

    public MainViewModel(ILogger logger)
    {
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            switch (msg.Value)
            {
                case PageName.Connect:
                    IsConnectPage = true;
                    IsTablesPage = false;
                    IsTestingPage = false;
                    break;
                case PageName.Tables:
                    IsConnectPage = false;
                    IsTablesPage = true;
                    IsTestingPage = false;
                    break;
                case PageName.Testing:
                    IsConnectPage = false;
                    IsTablesPage = false;
                    IsTestingPage = true;
                    break;
                default:
                    logger.Error("Illegal value for PageName enum {PageName}", msg.Value);
                    break;
            }
        });

        WeakReferenceMessenger.Default.Register<MainViewModel, ExeFolderMessage>(this, (r, m) =>
        {
            m.Reply(r.ExeFolder);
        });

        WeakReferenceMessenger.Default.Register<MainViewModel, WindowHandleMessage>(this, (r, m) =>
        {
            m.Reply(r.HWnd);
        });

        WeakReferenceMessenger.Default.Register<MainViewModel, XamlRootMessage>(this, (r, m) =>
        {
            m.Reply(r.XamlRoot);
        });
    }
}