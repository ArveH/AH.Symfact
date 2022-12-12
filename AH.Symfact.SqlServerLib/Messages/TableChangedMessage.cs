namespace AH.Symfact.SqlServerLib.Messages;

public class TableChangedMessage : ValueChangedMessage<TableChanged>
{
    public TableChangedMessage(TableChanged value) : base(value)
    {
    }
}