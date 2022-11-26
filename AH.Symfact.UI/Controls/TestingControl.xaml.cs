using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
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