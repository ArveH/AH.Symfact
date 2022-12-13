using Microsoft.SqlServer.Management.Smo;

namespace AH.Symfact.UI.ViewModels;

public partial class CollectionViewModel: ObservableRecipient
{
    private readonly IMongoCollectionService _collectionService;
    private readonly ISymfactFileReader _fileReader;
    private readonly ILogger _logger;
    private CancellationTokenSource _cts;

    public CollectionViewModel(
        string name,
        IMongoCollectionService collectionService,
        ISymfactFileReader fileReader,
        ILogger logger)
    {
        _collectionService = collectionService;
        _fileReader = fileReader;
        CollectionName = name;
        _logger = logger.ForContext<CollectionViewModel>();
        _cts = new CancellationTokenSource();
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
        if (!MongoConstants.DataInfo.TryGetValue(CollectionName, 
                out (string filePath, string elementPath) pos))
        {
            _logger.Error("Can't get DataInfo for Collection '{CollectionName}'", 
                CollectionName);
            return;
        }

        try
        {
            await _collectionService.DeleteCollectionAsync(CollectionName, _cts.Token);
            var nodes = _fileReader.ReadFromFile(
                pos.filePath, pos.elementPath);
            var count = await _collectionService.InsertAsync(
                CollectionName,
                nodes,
                c =>
                {
                    DispatcherQueue?.TryEnqueue(() =>
                    {
                        Count = c;
                    });
                },
                _cts.Token);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"(Re)Create '{CollectionName}' failed");
        }
    }
}