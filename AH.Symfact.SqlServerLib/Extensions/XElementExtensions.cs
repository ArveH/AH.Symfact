namespace AH.Symfact.SqlServerLib.Extensions;

public static class XElementExtensions
{
    public static bool Is(this XElement xElem, string name)
    {
        return xElem.Name.LocalName == name;
    }
}