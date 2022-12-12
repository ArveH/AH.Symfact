using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using Microsoft.UI.Dispatching;

namespace AH.Symfact.UI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    public MainViewModel(ILogger logger)
    {
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
        WeakReferenceMessenger.Default.Register<MainViewModel, DispatcherQueueMessage>(this, (r, m) =>
        {
            m.Reply(r.DispatcherQueue);
        });
    }

    public DispatcherQueue? DispatcherQueue { get; set; }

    [ObservableProperty]
    private IntPtr _hWnd;
    [ObservableProperty]
    private XamlRoot? _xamlRoot;
    [ObservableProperty]
    private string _exeFolder = "";
}