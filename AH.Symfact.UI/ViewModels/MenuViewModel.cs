namespace AH.Symfact.UI.ViewModels;

public partial class MenuViewModel : ObservableRecipient
{
    private readonly ILogger _logger;

    public MenuViewModel(ILogger logger)
    {
        _logger = logger.ForContext<MenuControl>();
    }

    [RelayCommand]
    private void LoadConnectDetails()
    {
        _logger.Information("Loading Connect page details");
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.Connect));
    }

    [RelayCommand]
    private void LoadTablesDetails()
    {
        _logger.Information("Loading Tables page details");
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.SqlTables));
    }

    [RelayCommand]
    private void LoadTestingDetails()
    {
        _logger.Information("Loading Testing page details");
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.SqlTesting));
    }

    [RelayCommand]
    private void LoadCollectionsDetails()
    {
        _logger.Information("Loading Collection page details");
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.Collections));
    }
}