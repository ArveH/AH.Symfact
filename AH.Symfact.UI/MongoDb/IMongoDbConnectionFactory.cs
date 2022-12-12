using MongoDB.Driver;

namespace AH.Symfact.UI.MongoDb;

public interface IMongoDbConnectionFactory
{
    MongoDbConnectionString MongoDbConnectionString { get; }
    IMongoClient CreateClient();
    IMongoDatabase GetDatabase();
}