using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AH.Symfact.UI.Controls;

public sealed partial class XmlFileDetailsControl
{
    public XmlFileDetailsViewModel ViewModel { get; }

    public XmlFileDetailsControl()
    {
        ViewModel = App.Current.Services.GetService<XmlFileDetailsViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for XmlFileDetailsControl");
        InitializeComponent();
    }
}
