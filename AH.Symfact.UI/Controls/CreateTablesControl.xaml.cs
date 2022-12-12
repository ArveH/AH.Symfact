namespace AH.Symfact.UI.Controls;

public sealed partial class CreateTablesControl
{
    public CreateTablesViewModel ViewModel { get; }

    public CreateTablesControl(CreateTablesViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}