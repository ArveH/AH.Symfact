using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.UI.Xaml;

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
    }
}