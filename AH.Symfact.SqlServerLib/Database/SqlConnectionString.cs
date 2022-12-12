namespace AH.Symfact.SqlServerLib.Database;

public class SqlConnectionString
{
    private readonly ILogger _logger;

    public SqlConnectionString(
        IConfiguration config,
        ILogger logger)
    {
        _logger = logger;
        ConnectionString = config.GetConnectionString(SharedConstants.ConfigKey.SqlConnectionString);
    }

    private string? _connectionString;
    public string? ConnectionString
    {
        get => _connectionString;
        set
        {
            if (value != _connectionString)
            {
                _connectionString = value;
                _logger.Debug("SqlServer ConnectionString changed '{ConnectionString}'", value);
            }
        }
    }

    public bool IsValid
    {
        get
        {
            try
            {
                var _ = new SqlConnectionStringBuilder(ConnectionString);
                return true;
            }
            catch (Exception)
            {
                _logger.Error("Invalid SqlServer ConnectionString '{ConnectionString}'",
                    ConnectionString);
                return false;
            }
        }
    }

    public string? DatabaseName
    {
        get
        {
            try
            {
                var csBuilder = new SqlConnectionStringBuilder(ConnectionString);
                if (csBuilder.TryGetValue("Database", out var dbName))
                    return (string)dbName;
                return null;
            }
            catch (Exception)
            {
                _logger.Error("Could not get Database from ConnectionString '{ConnectionString}'",
                    ConnectionString);
                return null;
            }
        }
    }
}