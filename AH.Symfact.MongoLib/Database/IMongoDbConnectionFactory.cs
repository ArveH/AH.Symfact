namespace AH.Symfact.MongoLib.Database;

public interface IMongoDbConnectionFactory
{
    MongoDbConnectionString MongoDbConnectionString { get; }
    bool CanConnect { get; }
    IMongoClient CreateClient();
    IMongoDatabase GetDatabase();
}