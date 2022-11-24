using AH.Symfact.UI.Config;
using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using Windows.ApplicationModel;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.Services;

namespace AH.Symfact.UI;

public partial class App
{
    private Window? _window;

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = Services.GetService<MainWindow>();
        _window?.Activate();
    }

    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection services = new();
        IConfiguration config = ConfigurationHelper.GetConfiguration(
            "f40ef935-6add-41f0-8535-68a7f5f6f954",
            Package.Current.InstalledLocation.Path);
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        // Services
        services.AddSingleton(config);
        services.AddSingleton(Log.Logger);
        services.AddTransient<MainWindow>();
        services.AddSingleton(new DbConnectionString(config, Log.Logger));
        services.AddSingleton<IDbConnFactory, DbConnFactory>();
        services.AddSingleton<IDbCommands, DbCommands>();
        services.AddSingleton<ISchemaService, SchemaService>();
        services.AddSingleton<ITableService, TableService>();
        services.AddSingleton<ITaminoFileReader, TaminoFileReader>();
        services.AddSingleton<IXElementHelper, XElementHelper>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<MenuViewModel>();
        services.AddTransient<ConnectViewModel>();
        services.AddTransient<TablesViewModel>();
        services.AddTransient<CreateTablesViewModel>();

        return services.BuildServiceProvider();
    }
}