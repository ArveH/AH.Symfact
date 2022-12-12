namespace AH.Symfact.UI.ViewModels.Messages;

public class PartyLoadedMessage : ValueChangedMessage<int>
{
    public PartyLoadedMessage(int value) : base(value)
    {
    }
}