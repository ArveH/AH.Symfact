using AH.Symfact.MongoLib.Services;

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
        var collectionService = App.Current.Services.GetService<IMongoCollectionService>();
        ArgumentNullException.ThrowIfNull(nameof(collectionService));
        InitializeComponent();
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel("Contract", collectionService!, logger)));
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel("Party", collectionService!, logger)));
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel("OrganisationalPerson", collectionService!, logger)));
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel("NonXMLFull", collectionService!, logger)));

        Visibility = Visibility.Collapsed;
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            Visibility = msg.Value == PageName.Collections ? Visibility.Visible : Visibility.Collapsed;
        });
    }
}