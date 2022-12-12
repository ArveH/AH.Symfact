namespace AH.Symfact.SqlServerLib.Models;

public class TableRow
{
    public TableRow(string docName, XElement data)
    {
        DocName = docName;
        Data = data;
    }

    public string DocName { get; set; }
    public XElement Data { get; set; }
}