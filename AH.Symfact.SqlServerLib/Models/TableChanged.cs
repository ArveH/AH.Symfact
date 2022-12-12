namespace AH.Symfact.SqlServerLib.Models;

public class TableChanged
{
    public TableChanged(
        string tableName,
        TableAction action = TableAction.NoAction, 
        string? msg = null)
    {
        TableName = tableName;
        Action = action;
        Message = msg;
    }

    public string TableName { get; set; }
    public TableAction Action { get; set; }
    public string? Message { get; set; }
    public int Rows { get; set; }
}