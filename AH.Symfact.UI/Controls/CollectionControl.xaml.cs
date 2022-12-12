using AH.Symfact.UI.ViewModels;

namespace AH.Symfact.UI.Controls;

public sealed partial class CollectionControl
{
    public CollectionViewModel ViewModel { get; set; }

    public CollectionControl(CollectionViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        ViewModel.DispatcherQueue = this.DispatcherQueue;
    }
}