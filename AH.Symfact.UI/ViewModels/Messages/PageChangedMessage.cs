using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AH.Symfact.UI.ViewModels.Messages;

public class PageChangedMessage : ValueChangedMessage<PageName>
{
    public PageChangedMessage(PageName value) : base(value)
    {
    }
}