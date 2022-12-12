namespace AH.Symfact.UI.Services;

public class XElementHelper : IXElementHelper
{
    private readonly ILogger _logger;

    public XElementHelper(ILogger logger)
    {
        _logger = logger;
    }

    public bool Is(XElement xElem, string name, bool logError = true)
    {
        if (xElem.Is(name)) return true;

        if (logError)
        {
            _logger.Error("The <{ExpectedElementName}> element was not found. Found '{ElementName}' instead",
                name, xElem.Name);
        }
        return false;
    }

    public XElement? GetFirstChild(XElement xElem)
    {
        var child = xElem.Elements().FirstOrDefault();
        if (child == null)
        {
            _logger.Error("The <{ElementName}> element did not contain descendants",
                xElem.Name.LocalName);
            return null;
        }

        return child;
    }

    public XElement? GetSecondChild(XElement xElem)
    {
        try
        {
            return xElem.Elements().ElementAt(1);
        }
        catch (Exception)
        {
            _logger.Error("The <{ElementName}> element did not contain descendants",
                xElem.Name.LocalName);
            return null;
        }
    }

    public T? Deserialize<T>(XElement xElem)
    {
        var serializer = new XmlSerializer(typeof(T));
        return (T?)serializer.Deserialize(xElem.CreateReader());
    }
}