using AH.Symfact.MongoLib.Services;

namespace AH.Symfact.UI.ViewModels;

public partial class CollectionViewModel: ObservableRecipient
{
    private readonly IMongoCollectionService _collectionService;
    private readonly ILogger _logger;

    public CollectionViewModel(
        string name,
        IMongoCollectionService collectionService,
        ILogger logger)
    {
        _collectionService = collectionService;
        CollectionName = name;
        _logger = logger.ForContext<CollectionViewModel>();
    }

    [ObservableProperty] 
    private int _count;
    
    public DispatcherQueue? DispatcherQueue { get; set; }
    public string CollectionName { get; }
    public string ButtonName => "(Re)Create " + CollectionName;

    [ObservableProperty] 
    private int _progressDone;

    [RelayCommand]
    public async Task RecreateCollectionAsync()
    {
        var count = await _collectionService.InsertAsync(
            CollectionName + ".xml",
            "//C:Contract",
            CollectionName,
            c =>
            {
                Count = c;
            });
    }
}