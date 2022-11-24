using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

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
        TablesStack.Children.Add(new CreateTablesControl(ViewModel.ContractViewModel));
        TablesStack.Children.Add(new CreateTablesControl(ViewModel.PartyViewModel));
        TablesStack.Children.Add(new CreateTablesControl(ViewModel.OrgPersonViewModel));
    }
}