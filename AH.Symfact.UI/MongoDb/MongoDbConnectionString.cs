using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Serilog;

namespace AH.Symfact.UI.MongoDb;

public class MongoDbConnectionString
{
    private readonly ILogger _logger;

    public MongoDbConnectionString(
        IConfiguration config,
        ILogger logger)
    {
        _logger = logger.ForContext<MongoDbConnectionString>();
        ConnectionString = config.GetConnectionString(SymfactConstants.ConfigKey.MongoConnectionString);
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
}