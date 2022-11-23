namespace AH.Symfact.UI.Models;

public class ContractChanged
{
    public ContractChanged(TableAction action = TableAction.NoAction, string? msg = null)
    {
        Action = action;
        Message = msg;
    }

    public TableAction Action { get; set; }
    public string? Message { get; set; }
}