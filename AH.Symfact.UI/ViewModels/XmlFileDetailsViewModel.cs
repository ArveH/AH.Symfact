using AH.Symfact.UI.Models;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AH.Symfact.UI.ViewModels;

public partial class XmlFileDetailsViewModel : ObservableRecipient
{
    [ObservableProperty]
    private XmlFileInfo? _xmlFileInfo;

    public XmlFileDetailsViewModel()
    {
        WeakReferenceMessenger.Default.Register<XmlFileLoadedMessage>(this, (_, msg) =>
        {
            XmlFileInfo = msg.Value;
        });
    }
}