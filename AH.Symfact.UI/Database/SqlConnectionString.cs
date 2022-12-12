using Microsoft.Extensions.Configuration;
using Serilog;

namespace AH.Symfact.UI.Database;

public class SqlConnectionString
{
    private readonly ILogger _logger;

    public SqlConnectionString(
        IConfiguration config,
        ILogger logger)
    {
        _logger = logger;
        ConnectionString = config.GetConnectionString(SymfactConstants.ConfigKey.SqlConnectionString);
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
                _logger.Debug("ConnectionString changed '{ConnectionString}'", value);
            }
        }
    }
}