// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using AH.Symfact.UI.Config;
using AH.Symfact.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using Windows.ApplicationModel;

namespace AH.Symfact.UI;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
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
        services.AddSingleton(Log.Logger);
        services.AddTransient<MainWindow>();

        // ViewModels
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }
}