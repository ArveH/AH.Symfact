using System;
using AH.Symfact.UI.ViewModels;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AH.Symfact.UI.Controls;

public sealed partial class CollectionsControl
{
    public CollectionsViewModel ViewModel { get; }

    public CollectionsControl()
    {
        ViewModel = App.Current.Services.GetService<CollectionsViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for CollectionsControl");
        var logger = App.Current.Services.GetService<ILogger>() ?? Log.Logger;
        InitializeComponent();
        CollectionStack.Children.Add(new CollectionControl(new CollectionViewModel("Contract", logger)));
        CollectionStack.Children.Add(new CollectionControl(new CollectionViewModel("Party", logger)));
        CollectionStack.Children.Add(new CollectionControl(new CollectionViewModel("OrganisationalPerson", logger)));
        CollectionStack.Children.Add(new CollectionControl(new CollectionViewModel("NonXMLFull", logger)));

        Visibility = Visibility.Collapsed;
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            Visibility = msg.Value == PageName.Collections ? Visibility.Visible : Visibility.Collapsed;
        });
    }
}