namespace AH.Symfact.SqlServerLib.Services;

public interface ITaminoFileReader
{
    string? GetName(XElement xElem);
    IReadOnlyCollection<TableRow> SplitRequests(XElement xElem);
    IReadOnlyCollection<TableRow> SplitNonXml(XElement xElem);
}