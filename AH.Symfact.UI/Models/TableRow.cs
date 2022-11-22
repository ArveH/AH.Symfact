namespace AH.Symfact.UI.Models;

public class TableRow
{
    public TableRow(string docName, string data)
    {
        DocName = docName;
        Data = data;
    }

    public string DocName { get; set; }
    public string Data { get; set; }
}