namespace AH.Symfact.MongoLib.Database;

public interface IMongoDbConnectionFactory
{
    MongoDbConnectionString MongoDbConnectionString { get; }
    IMongoClient CreateClient();
    IMongoDatabase GetDatabase();
}