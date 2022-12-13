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
        var fileReader = App.Current.Services.GetService<ISymfactFileReader>();
        ArgumentNullException.ThrowIfNull(nameof(collectionService));
        InitializeComponent();
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel(
                SharedConstants.Name.Contract, 
                collectionService!, 
                fileReader!, 
                logger)));
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel(
                SharedConstants.Name.Party, 
                collectionService!, 
                fileReader!, 
                logger)));
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel(
                SharedConstants.Name.OrganisationalPerson, 
                collectionService!, 
                fileReader!, 
                logger)));
        CollectionStack.Children.Add(new CollectionControl(
            new CollectionViewModel(
                SharedConstants.Name.NonXMLFull, 
                collectionService!, 
                fileReader!, 
                logger)));

        Visibility = Visibility.Collapsed;
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            Visibility = msg.Value == PageName.Collections ? Visibility.Visible : Visibility.Collapsed;
        });
    }
}