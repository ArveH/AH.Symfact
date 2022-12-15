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

    [ObservableProperty] 
    private bool _isCreateIndexActive;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RecreateCollectionCommand))]
    [NotifyCanExecuteChangedFor(nameof(RecreateTextIndexCommand))]
    private bool _isIdle = true;

    [ObservableProperty] 
    private string _textIndexFields = "{\"$**\": \"text\"}";

    public DispatcherQueue? DispatcherQueue { get; set; }
    public string CollectionName { get; }

    [ObservableProperty] 
    private int _progressDone;

    private bool CanCommandsExecute() => IsIdle;

    [RelayCommand(CanExecute = nameof(CanCommandsExecute))]
    public async Task RecreateCollectionAsync()
    {
        await Task.Run(async () =>
        {
            try
            {
                DispatcherQueue?.TryEnqueue(() => { IsIdle = false; });
                if (!MongoConstants.DataInfo.TryGetValue(CollectionName,
                        out (string filePath, string elementPath, string nsToRemove) pos))
                {
                    _logger.Error("Can't get DataInfo for Collection '{CollectionName}'",
                        CollectionName);
                    return;
                }

                await _collectionService.DeleteCollectionAsync(CollectionName, _cts.Token);
                var nodes = _fileReader.ReadFromFile(
                    pos.filePath, pos.elementPath);
                DispatcherQueue?.TryEnqueue(() =>
                {
                    ProgressDone = 0;
                    Count = nodes.Count;
                });
                await _collectionService.InsertAsync(
                    CollectionName,
                    pos.nsToRemove,
                    nodes,
                    c => { DispatcherQueue?.TryEnqueue(() => { ProgressDone = c * 100 / nodes.Count; }); },
                    _cts.Token);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"(Re)Create '{CollectionName}' failed");
            }
            finally
            {
                DispatcherQueue?.TryEnqueue(() => { IsIdle = true; });
            }
        });
    }

    [RelayCommand(CanExecute = nameof(CanCommandsExecute))]
    public async Task RecreateTextIndexAsync()
    {
        await Task.Run(async () =>
        {
            try
            {
                DispatcherQueue?.TryEnqueue(() =>
                {
                    IsCreateIndexActive = true;
                    IsIdle = false;
                });
                await _collectionService.DropTextIndexAsync(CollectionName);
                await _collectionService.CreateTextIndexAsync(CollectionName, TextIndexFields);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Recreating text index on collection '{CollectionName}' for fields {Fields} failed",
                    CollectionName, TextIndexFields);
            }
            finally
            {
                DispatcherQueue?.TryEnqueue(() =>
                {
                    IsCreateIndexActive = false;
                    IsIdle = true;
                });
            }
        });
    }
}