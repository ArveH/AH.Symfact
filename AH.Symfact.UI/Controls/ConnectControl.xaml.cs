using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AH.Symfact.UI.Controls;

public sealed partial class ConnectControl
{
    public ConnectViewModel ViewModel { get; }

    public ConnectControl()
    {
        ViewModel = App.Current.Services.GetService<ConnectViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for ConnectControl");
        InitializeComponent();
    }
}