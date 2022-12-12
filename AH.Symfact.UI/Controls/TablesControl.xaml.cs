using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.UI.Xaml;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AH.Symfact.UI.Controls;

public sealed partial class TablesControl
{
    public TablesViewModel ViewModel { get; }

    public TablesControl()
    {
        ViewModel = App.Current.Services.GetService<TablesViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for TablesControl");
        InitializeComponent();
        TablesStack.Children.Add(new CreateTablesControl(ViewModel.ContractViewModel){HorizontalAlignment = HorizontalAlignment.Stretch});
        TablesStack.Children.Add(new CreateTablesControl(ViewModel.PartyViewModel){HorizontalAlignment = HorizontalAlignment.Stretch});
        TablesStack.Children.Add(new CreateTablesControl(ViewModel.OrgPersonViewModel){HorizontalAlignment = HorizontalAlignment.Stretch});

        Visibility = Visibility.Collapsed;
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            Visibility = msg.Value == PageName.SqlTables ? Visibility.Visible : Visibility.Collapsed;
        });
    }
}