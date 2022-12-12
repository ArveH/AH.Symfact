namespace AH.Symfact.UI.ViewModels.Messages;

public class TableChangedMessage : ValueChangedMessage<TableChanged>
{
    public TableChangedMessage(TableChanged value) : base(value)
    {
    }
}