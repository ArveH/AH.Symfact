using System.Collections;

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

    public async Task DeleteCollectionAsync(
        string collectionName,
        CancellationToken ct)
    {
        _logger.Information("Deleting collection '{CollectionName}'...",
            collectionName);
        var database = _connectionFactory.GetDatabase();
        await database.DropCollectionAsync(collectionName, ct);
        _logger.Information("Collection '{CollectionName}' deleted",
            collectionName);
    }

    public async Task CreateTextIndexAsync(
        string collectionName,
        string fields)
    {
        _logger.Information("Creating text index for fields {Fields} on collection '{CollectionName}'...",
            fields, collectionName);
        var database = _connectionFactory.GetDatabase();
        var collection = database.GetCollection<BsonDocument>(collectionName);
        var name = await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Text("$**")));
        _logger.Information("Created text index on collection '{CollectionName}' for fields {Fields}",
            collectionName, fields);
    }

    public async Task<int> InsertAsync(
        string collectionName,
        string nsToRemove,
        XmlNodeList nodes,
        Action<int> progress,
        CancellationToken ct)
    {
        var nodesCount = nodes.Count;
        _logger.Information("Starting to insert {Count} documents into '{CollectionName}'...",
            nodesCount, collectionName);
        var counter = 0;
        try
        {
            var database = _connectionFactory.GetDatabase();
            var collection = database.GetCollection<BsonDocument>(collectionName);

            foreach (XmlNode party in nodes)
            {
                if (ct.IsCancellationRequested)
                {
                    _logger.Warning("Inserting documents into '{CollectionName}' was cancelled after {Count} inserts",
                        collectionName, counter);
                    return counter;
                }
                var json = JsonConvert.SerializeXmlNode(party, new Newtonsoft.Json.Formatting(), true)
                    .Replace("@ID", "_id");
                if (!string.IsNullOrWhiteSpace(nsToRemove))
                {
                    json = json.Replace(nsToRemove, string.Empty);
                }
                var bDoc = BsonDocument.Parse(json);
                await collection.InsertOneAsync(bDoc, null, ct);
                counter++;
                progress(counter);
            }

            _logger.Information("Inserted {Count} documents into '{CollectionName}'",
                nodesCount, collectionName);
            return counter;
        }
        catch (Exception)
        {
            _logger.Error("Inserting documents into '{CollectionName}' failed after {Count} inserts",
                collectionName, counter);
            throw;
        }
    }
}