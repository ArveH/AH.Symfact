using AH.Symfact.UI.ViewModels;
using Microsoft.UI.Xaml;
using Serilog;
using System.Threading.Tasks;

namespace AH.Symfact.UI;

public sealed partial class MainWindow : Window
{
    private readonly ILogger _logger;

    public MainWindow(
        MainViewModel viewModel,
        ILogger logger)
    {
        _logger = logger.ForContext<MainWindow>();
        InitializeComponent();
        ViewModel = viewModel;
        Root.Loaded += Root_Loaded;
    }

    public MainViewModel ViewModel { get; }

    private async void Root_Loaded(object sender, RoutedEventArgs e)
    {
        _logger.Information("Started Symfact.UI");
        await Task.CompletedTask;
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        Log.CloseAndFlush();
    }
}