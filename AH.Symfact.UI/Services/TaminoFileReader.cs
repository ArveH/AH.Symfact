using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AH.Symfact.UI.Models;
using Serilog;

namespace AH.Symfact.UI.Services;

public class TaminoFileReader : ITaminoFileReader
{
    private readonly IXElementHelper _xElementHelper;
    private readonly ILogger _logger;

    public TaminoFileReader(
        IXElementHelper xElementHelper,
        ILogger logger)
    {
        _xElementHelper = xElementHelper;
        _logger = logger.ForContext<TaminoFileReader>();
    }

    public string? GetName(XElement xElem)
    {
        if (!_xElementHelper.Is(xElem, "request")) return null;

        var objectElem = GetObjectElem(xElem);
        if (objectElem == null) return null;

        if (GetDocNameAttr(objectElem) == null) return null;

        var mainElement = GetMainElem(objectElem);
        if (mainElement == null) return null;

        return mainElement.Name.LocalName;
    }

    public IReadOnlyCollection<TableRow> SplitRequests(XElement xElem)
    {
        var elementCounter = 0;
        var errorCounter = 0;
        var elementName = string.Empty;

        if (!_xElementHelper.Is(xElem, "request")) return new List<TableRow>();

        var tableRows = new List<TableRow>();
        foreach (var objectElem in xElem.Elements())
        {
            if (!_xElementHelper.Is(objectElem, "object"))
            {
                errorCounter++;
                continue;
            }

            var docName = GetDocNameAttr(objectElem);
            if (docName == null)
            {
                errorCounter++;
                continue;
            }

            var mainElement = GetMainElem(objectElem);
            if (mainElement == null)
            {
                errorCounter++;
                continue;
            }

            if (elementCounter == 0)
            {
                elementName = mainElement.Name.LocalName;
                _logger.Information("Getting {ElementName}s...", elementName);
            }

            tableRows.Add(new TableRow(docName, mainElement.ToString()));
            elementCounter++;
        }

        _logger.Information("Found {ElementCounter} {ElementName}s",
            elementCounter, elementName);
        if (errorCounter > 0)
        {
            _logger.Warning("{ErrorCounter} {ElementName}s failed to load",
                errorCounter, elementName);
        }

        return tableRows;
    }

    public IReadOnlyCollection<TableRow> SplitNonXml(XElement xElem)
    {
        var elementCounter = 0;
        var errorCounter = 0;
        var elementName = "nonXml";

        if (!_xElementHelper.Is(xElem, "Root")) return new List<TableRow>();

        var tableRows = new List<TableRow>();
        foreach (var nonXmlElem in xElem.Elements())
        {
            if (!_xElementHelper.Is(nonXmlElem, "nonXML"))
            {
                errorCounter++;
                continue;
            }

            var docName = GetDocNameAttr(nonXmlElem);
            if (docName == null)
            {
                errorCounter++;
                continue;
            }

            if (elementCounter == 0)
            {
                _logger.Information("Getting {ElementName}s...", elementName);
            }

            tableRows.Add(new TableRow(docName, nonXmlElem.ToString()));
            elementCounter++;
        }

        _logger.Information("Found {ElementCounter} {ElementName}s",
            elementCounter, elementName);
        if (errorCounter > 0)
        {
            _logger.Warning("{ErrorCounter} {ElementName}s failed to load",
                errorCounter, elementName);
        }

        return tableRows;
    }

    private string? GetDocNameAttr(XElement objectElem)
    {
        var docNameAttr = objectElem
            .Attributes()
            .FirstOrDefault(a => a.Name.LocalName == "docname");
        if (docNameAttr == null)
        {
            _logger.Error("The <docname> attribute was not found on the <object> element");
            return null;
        }

        return docNameAttr.Value;
    }

    private XElement? GetMainElem(XElement objectElem)
    {
        var mainElement = _xElementHelper.GetFirstChild(objectElem);
        if (mainElement == null) return null;

        if (_xElementHelper.Is(mainElement, "documentprolog", false))
        {
            mainElement = _xElementHelper.GetSecondChild(objectElem);
            if (mainElement == null) return null;
        }

        return mainElement;
    }

    private XElement? GetObjectElem(XElement xElem)
    {
        var objectElem = _xElementHelper.GetFirstChild(xElem);
        if (objectElem == null) return null;

        return _xElementHelper.Is(objectElem, "object") ? objectElem : null;
    }
}