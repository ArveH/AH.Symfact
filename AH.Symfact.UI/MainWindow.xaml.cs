using AH.Symfact.UI.ViewModels;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using System.Threading.Tasks;
using WinRT.Interop;

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

        try
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            if (hWnd != IntPtr.Zero)
            {
                ViewModel.HWnd = hWnd;
            }
            else
            {
                Console.WriteLine("Can't get WindowHandle");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Can't set WindowHandle: " + ex);
        }
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