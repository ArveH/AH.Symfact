namespace AH.Symfact.MongoLib.Database;

public class MongoDbConnectionString
{
    private readonly ILogger _logger;

    public MongoDbConnectionString(
        IConfiguration config,
        ILogger logger)
    {
        _logger = logger.ForContext<MongoDbConnectionString>();
        ConnectionString = config.GetConnectionString(SharedConstants.ConfigKey.MongoConnectionString);
    }

    public string? DatabaseName => MongoUrl.Create(ConnectionString).DatabaseName;

    private string? _connectionString;
    public string? ConnectionString
    {
        get => _connectionString;
        set
        {
            if (value != _connectionString)
            {
                _connectionString = value;
                _logger.Debug("MongoDb ConnectionString changed '{ConnectionString}'", value);
            }
        }
    }

    public bool IsValid
    {
        get
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DatabaseName))
                {
                    _logger.Error("Database name missing from MongoDb connection string.");
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                _logger.Error("Invalid MongoDb ConnectionString '{ConnectionString}'",
                    ConnectionString);
                return false;
            }
        }
    }
}