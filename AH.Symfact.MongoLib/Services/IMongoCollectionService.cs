namespace AH.Symfact.MongoLib.Services;

public interface IMongoCollectionService
{
    Task<int> InsertAsync(string collectionName,
        string nsToRemove,
        XmlNodeList nodes,
        Action<int> progress,
        CancellationToken ct);

    Task DeleteCollectionAsync(
        string collectionName,
        CancellationToken ct);

    Task CreateTextIndexAsync(string collectionName, string fieldsStr);
    Task DropTextIndexAsync(string collectionName);

    int BulkInsert(
        string collectionName,
        string nsToRemove,
        XmlNodeList nodes,
        CancellationToken ct);
}