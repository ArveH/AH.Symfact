using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AH.Symfact.UI.Controls;

public sealed partial class CreateTablesControl
{
    public CreateTablesViewModel ViewModel { get; }

    public CreateTablesControl()
    {
        ViewModel = App.Current.Services.GetService<CreateTablesViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for CreateTablesControl");
        InitializeComponent();
    }
}