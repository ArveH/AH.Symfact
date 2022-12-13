namespace AH.Symfact.MongoLib.Services;

public class SymfactFileReader : ISymfactFileReader
{
    private readonly ILogger _logger;

    public SymfactFileReader(
        ILogger logger)
    {
        _logger = logger.ForContext<SymfactFileReader>();
    }

    public XmlNodeList ReadFromFile(
        string xmlDataFile,
        string nodesPath)
    {
        var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var filePath = Path.Combine(dataPath, xmlDataFile);
        _logger.Information("Starting to read from file '{FilePath}'...",
            filePath);

        var xDoc = new XmlDocument();
        xDoc.Load(filePath);

        var nodes = xDoc.SelectNodes(nodesPath, NamespaceManager);
        if (nodes == null)
        {
            throw new ArgumentNullException(nameof(nodes),
                $"Couldn't find nodes at path '{nodesPath}' in file '{xmlDataFile}'");
        }

        _logger.Information("Found {Count} nodes at '{NodesPath}' file '{FilePath}'...",
            nodes.Count, nodesPath, filePath);
        return nodes;
    }

    private static XmlNamespaceManager? _namespaceManager;
    public static XmlNamespaceManager NamespaceManager
    {
        get
        {
            if (_namespaceManager == null)
            {
                _namespaceManager = new XmlNamespaceManager(new XmlDocument().NameTable);

                _namespaceManager.AddNamespace("C", "symfact/Contract");
                _namespaceManager.AddNamespace("P", "symfact/Party");
                _namespaceManager.AddNamespace("LE", "contractX/LegalEntity");
                _namespaceManager.AddNamespace("ctxO", "contractX/contractXOrganisation");
                _namespaceManager.AddNamespace("UP", "symfact/UserProfile");


                _namespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                _namespaceManager.AddNamespace("meta", "http://openoffice.org/2000/meta");
                _namespaceManager.AddNamespace("office", "http://openoffice.org/2000/office");

                _namespaceManager.AddNamespace("xq", "http://namespaces.softwareag.com/tamino/XQuery/result");
                _namespaceManager.AddNamespace("xql", "http://metalab.unc.edu/xql/");
                _namespaceManager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
                _namespaceManager.AddNamespace("ino", "http://namespaces.softwareag.com/tamino/response2");
                _namespaceManager.AddNamespace("tf", "http://namespaces.softwareag.com/tamino/TaminoFunction'");
            }
            return _namespaceManager;
        }
    }
}