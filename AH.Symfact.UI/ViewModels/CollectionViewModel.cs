using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Serilog;

namespace AH.Symfact.UI.ViewModels;

public partial class CollectionViewModel: ObservableRecipient
{
    private readonly ILogger _logger;

    public CollectionViewModel(
        string name,
        ILogger logger)
    {
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
        ProgressDone = 0;
        Count = 0;
        for (int i = 0; i < 10; i++)
        {
            ProgressDone += 10;
            Count += 10;
            await Task.Delay(1000);
        }

        ProgressDone = 100;
    }
}