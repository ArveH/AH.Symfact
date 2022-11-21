using Serilog;

namespace AH.Symfact.UI.Database;

public class DbConnFactory : IDbConnFactory
{
    private readonly ILogger _logger;

    public DbConnFactory(
        DbConnectionString dbConnectionString,
        ILogger logger)
    {
        DbConnectionString = dbConnectionString;
        _logger = logger;
    }

    public DbConnectionString DbConnectionString { get; }

    public IDbConn CreateConnection()
    {
        return new DbConn(DbConnectionString, _logger);
    }
}