using System.Xml.Linq;

namespace AH.Symfact.UI.Extensions;

public static class XElementExtensions
{
    public static bool Is(this XElement xElem, string name)
    {
        return xElem.Name.LocalName == name;
    }
}