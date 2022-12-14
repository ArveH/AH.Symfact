namespace AH.Symfact.UI;

public sealed partial class MainWindow
{
    private readonly ILogger _logger;

    public MainWindow(
        MainViewModel viewModel,
        ILogger logger)
    {
        _logger = logger.ForContext<MainWindow>();
        InitializeComponent();
        Title = "Symfact Database Utilities";
        ViewModel = viewModel;
        Root.Loaded += Root_Loaded;
        ViewModel.ExeFolder = Windows.ApplicationModel.Package.Current.InstalledPath;

        SetWindowHandle();
    }

    public MainViewModel ViewModel { get; }

    private async void Root_Loaded(object sender, RoutedEventArgs e)
    {
        _logger.Information("Started Symfact.UI");
        SetMainXamlRootHandle();
        ViewModel.DispatcherQueue = this.DispatcherQueue;
        WeakReferenceMessenger.Default.Send(new PageChangedMessage(PageName.Connect));
        await Task.CompletedTask;
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        Log.CloseAndFlush();
    }

    private void SetMainXamlRootHandle()
    {
        try
        {
            ViewModel.XamlRoot = Content.XamlRoot;
            if (ViewModel.XamlRoot == null)
            {
                _logger.Error("Can't get XamlRoot");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't set XamlRoot");
        }
    }

    private void SetWindowHandle()
    {
        try
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            if (hWnd != IntPtr.Zero)
            {
                ViewModel.HWnd = hWnd;
            }
            else
            {
                _logger.Error("Can't get WindowHandle");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't set WindowHandle");
        }
    }
}