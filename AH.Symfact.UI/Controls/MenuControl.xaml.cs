using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AH.Symfact.UI.Controls;

public sealed partial class MenuControl
{
    public MenuViewModel ViewModel { get; }

    public MenuControl()
    {
        ViewModel = App.Current.Services.GetService<MenuViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for MenuControl");
        InitializeComponent();
    }
}