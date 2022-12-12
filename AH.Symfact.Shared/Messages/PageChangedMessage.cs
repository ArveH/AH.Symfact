namespace AH.Symfact.Shared.Messages;

public class PageChangedMessage : ValueChangedMessage<PageName>
{
    public PageChangedMessage(PageName value) : base(value)
    {
    }
}