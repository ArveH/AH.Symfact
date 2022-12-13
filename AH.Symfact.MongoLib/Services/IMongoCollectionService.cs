namespace AH.Symfact.MongoLib.Services;

public interface IMongoCollectionService
{
    Task<int> InsertAsync(
        string xmlDataFile,
        string nodesPath,
        string collectionName,
        Action<int> progress);
}