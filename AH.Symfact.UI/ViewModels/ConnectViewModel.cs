namespace AH.Symfact.UI.ViewModels;

public partial class ConnectViewModel : ObservableObject
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IMongoDbConnectionFactory _mongoDbConnectionFactory;
    private readonly ILogger _logger;

    public ConnectViewModel(
        ISqlConnectionFactory sqlConnectionFactory,
        IMongoDbConnectionFactory mongoDbConnectionFactory,
        ILogger logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _mongoDbConnectionFactory = mongoDbConnectionFactory;
        _logger = logger.ForContext<ConnectViewModel>();
        _sqlConnectionString = _sqlConnectionFactory.SqlConnectionString.ConnectionString ?? "";
        _mongoDbConnectionString = _mongoDbConnectionFactory.MongoDbConnectionString.ConnectionString ?? "";
    }

    [ObservableProperty]
    private string _sqlConnectionString;
    partial void OnMongoDbConnectionStringChanged(string value)
    {
        _mongoDbConnectionFactory.MongoDbConnectionString.ConnectionString = value;
    }

    [ObservableProperty]
    private string _mongoDbConnectionString;
    partial void OnSqlConnectionStringChanging(string value)
    {
        _sqlConnectionFactory.SqlConnectionString.ConnectionString = value;
    }

    [ObservableProperty]
    private string _connectionStatus = "Ready...";

    [RelayCommand]
    public async Task ConnectSqlAsync()
    {
        if (!_sqlConnectionFactory.SqlConnectionString.IsValid)
        {
            ConnectionStatus = "Invalid SqlServer ConnectionString";
            return;
        }

        _logger.Information("Connecting...");
        ConnectionStatus = "Connecting...";
        try
        {
            await using var dbConn = _sqlConnectionFactory.CreateConnection();

            if (await dbConn.ConnectAsync())
            {
                var database = dbConn.DbName;
                if (string.IsNullOrWhiteSpace(database))
                {
                    _logger.Error("Can't get DbName from '{ConnectionString}'",
                        SqlConnectionString);
                    ConnectionStatus = "Can't get DbName";
                    return;
                }

                _logger.Information("Connection to SqlServer '{Database}' worked", database);
                ConnectionStatus = $"Connection to SqlServer '{database}' worked";
            }
            else
            {
                var dbName = _sqlConnectionFactory.SqlConnectionString.DatabaseName ?? "<null>";
                _logger.Error("Failed to connect to database SqlServer '{Database}'", dbName);
                ConnectionStatus = $"Failed to connect to SqlServer database '{dbName}'";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Connection to SqlServer failed");
            ConnectionStatus = ex.FlattenMessages();
        }
    }

    [RelayCommand]
    public async Task ConnectMongoAsync()
    {
        try
        {
            _logger.Information("Connecting...");
            ConnectionStatus = "Connecting...";

            if (!_mongoDbConnectionFactory.MongoDbConnectionString.IsValid)
            {
                ConnectionStatus = "Database name missing from MongoDb connection string.";
                return;
            }

            if (!_mongoDbConnectionFactory.CanConnect)
            {
                ConnectionStatus = "Connection to MongoDb failed";
                return;
            }

            var dbName = _mongoDbConnectionFactory.MongoDbConnectionString.DatabaseName;
            var db = _mongoDbConnectionFactory.GetDatabase();
            var collections = await db.ListCollectionNamesAsync();
            if (!await collections.MoveNextAsync())
            {
                _logger.Information("Connection to MongoDb '{Database}' worked, the the database was empty", dbName);
                ConnectionStatus = $"Connection to MongoDb '{dbName}' worked, the the database was empty";
            }
            else
            {
                _logger.Information("Connection to MongoDb '{Database}' worked", dbName);
                ConnectionStatus = $"Connection to MongoDb '{dbName}' worked";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Connection to MongoDb failed");
            ConnectionStatus = ex.FlattenMessages();
        }
    }
}