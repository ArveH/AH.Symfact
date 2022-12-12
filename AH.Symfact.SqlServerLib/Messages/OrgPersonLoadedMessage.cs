namespace AH.Symfact.SqlServerLib.Messages;

public class OrgPersonLoadedMessage : ValueChangedMessage<int>
{
    public OrgPersonLoadedMessage(int value) : base(value)
    {
    }
}