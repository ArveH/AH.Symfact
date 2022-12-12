namespace AH.Symfact.SqlServerLib.Messages;

public class PartyLoadedMessage : ValueChangedMessage<int>
{
    public PartyLoadedMessage(int value) : base(value)
    {
    }
}