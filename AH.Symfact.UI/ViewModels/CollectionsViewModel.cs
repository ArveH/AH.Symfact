using AH.Symfact.MongoLib.Services;

namespace AH.Symfact.UI.ViewModels;

public class CollectionsViewModel: ObservableRecipient
{
    private readonly ILogger _logger;

    public CollectionsViewModel(
        ILogger logger)
    {
        _logger = logger.ForContext<CollectionsViewModel>();
    }
   
}