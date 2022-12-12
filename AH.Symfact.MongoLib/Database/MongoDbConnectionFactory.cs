namespace AH.Symfact.MongoLib.Database;

public class MongoDbConnectionFactory : IMongoDbConnectionFactory
{
    private readonly ILogger _logger;

    public MongoDbConnectionFactory(
        MongoDbConnectionString mongoDbConnectionString,
        ILogger logger)
    {
        _logger = logger.ForContext<MongoDbConnectionFactory>();
        MongoDbConnectionString = mongoDbConnectionString;
    }

    public MongoDbConnectionString MongoDbConnectionString { get; }

    public IMongoClient CreateClient()
    {
        return new MongoClient(MongoDbConnectionString.ConnectionString);
    }

    public IMongoDatabase GetDatabase()
    {
        return CreateClient().GetDatabase(MongoDbConnectionString.DatabaseName);
    }

    public bool CanConnect
    {
        get
        {
            try
            {
                var db = GetDatabase();
                var res = db.RunCommand((Command<BsonDocument>)"{ping:1}");
                if (res["ok"].ToInt32() == 1) return true;

                _logger.Error("Connection to MongoDb failed");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not connect with ConnectionString '{ConnectionString}'",
                    MongoDbConnectionString.ConnectionString);
                return false;
            }
        }
    }
}