namespace AH.Symfact.MongoLib.Services;

public interface IMongoCollectionService
{
    Task<int> InsertAsync(
        string collectionName,
        XmlNodeList nodes,
        Action<int> progress,
        CancellationToken ct);

    Task DeleteCollectionAsync(
        string collectionName,
        CancellationToken ct);
}