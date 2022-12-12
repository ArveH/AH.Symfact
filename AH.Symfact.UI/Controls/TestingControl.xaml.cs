using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.UI.Xaml;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;

namespace AH.Symfact.UI.Controls;

public sealed partial class TestingControl
{
    public TestingViewModel ViewModel { get; }

    public TestingControl()
    {
        ViewModel = App.Current.Services.GetService<TestingViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for TestingControl");
        this.InitializeComponent();
        ViewModel.Dispatcher = this.Dispatcher;
        ViewModel.DispatcherQueue = this.DispatcherQueue;

        Visibility = Visibility.Collapsed;
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            Visibility = msg.Value == PageName.SqlTesting ? Visibility.Visible : Visibility.Collapsed;
        });
    }

    private void OnDropDownOpened(object? sender, object e)
    {
        ViewModel.SelectQueryFile();
    }

    private void OnQueryFileChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            ViewModel.QueryFileChanged(e.AddedItems[0].ToString());
        }
    }

    private void OnTableTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            ViewModel.TableTypeChanged();
        }
    }
}