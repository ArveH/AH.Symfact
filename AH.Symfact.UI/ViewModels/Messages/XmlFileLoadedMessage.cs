using AH.Symfact.UI.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AH.Symfact.UI.ViewModels.Messages;

public class XmlFileLoadedMessage : ValueChangedMessage<XmlFileInfo>
{
    public XmlFileLoadedMessage(XmlFileInfo value) : base(value)
    {
    }
}