namespace AH.Symfact.UI.Services;

public interface IXElementHelper
{
    bool Is(XElement xElem, string name, bool logError = true);
    XElement? GetFirstChild(XElement xElem);
    XElement? GetSecondChild(XElement xElem);
    T? Deserialize<T>(XElement xElem);
}