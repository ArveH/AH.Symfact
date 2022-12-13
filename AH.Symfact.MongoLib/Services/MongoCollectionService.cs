namespace AH.Symfact.MongoLib.Services;

public class MongoCollectionService : IMongoCollectionService
{
    private readonly IMongoDbConnectionFactory _connectionFactory;
    private readonly ILogger _logger;

    public MongoCollectionService(
        IMongoDbConnectionFactory connectionFactory,
        ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger.ForContext<MongoCollectionService>();
    }

    public async Task<int> InsertAsync(
        string xmlDataFile,
        string nodesPath,
        string collectionName,
        Action<int> progress)
    {
        var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var filePath = Path.Combine(dataPath, xmlDataFile);
        _logger.Information("Starting to load from file '{FilePath}'...",
            filePath);
        var count = 0;

        try
        {
            var database = _connectionFactory.GetDatabase();
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var xDoc = new XmlDocument();
            xDoc.Load(filePath);

            var nodes = xDoc.SelectNodes(nodesPath, NamespaceManager);
            if (nodes == null)
            {
                _logger.Error("Couldn't find nodes at path '{NodesPath}'", nodesPath);
                return count;
            }
            foreach (XmlNode party in nodes)
            {
                var json = JsonConvert.SerializeXmlNode(party, new Newtonsoft.Json.Formatting(), true)
                    .Replace("@ID", "_id");
                var bDoc = BsonDocument.Parse(json);
                await collection.InsertOneAsync(bDoc);
                count++;
                progress(count);
            }

            _logger.Information("Loaded {Count} documents from file '{FilePath}'",
                count, filePath);
            return count;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Reading {FilePath} failed", filePath);
            return 0;
        }
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