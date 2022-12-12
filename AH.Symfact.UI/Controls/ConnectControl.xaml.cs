namespace AH.Symfact.UI.Controls;

public sealed partial class ConnectControl
{
    public ConnectViewModel ViewModel { get; }

    public ConnectControl()
    {
        ViewModel = App.Current.Services.GetService<ConnectViewModel>()
                    ?? throw new ApplicationException(
                        "Can't get ViewModel for ConnectControl");
        InitializeComponent();

        Visibility = Visibility.Collapsed;
        WeakReferenceMessenger.Default.Register<PageChangedMessage>(this, (_, msg) =>
        {
            Visibility = msg.Value == PageName.Connect ? Visibility.Visible : Visibility.Collapsed;
        });
    }
}