using Serilog;

namespace AH.Symfact.UI.SqlServer;

public class DbConnFactory : IDbConnFactory
{
    private readonly ILogger _logger;

    public DbConnFactory(
        SqlConnectionString sqlConnectionString,
        ILogger logger)
    {
        SqlConnectionString = sqlConnectionString;
        _logger = logger;
    }

    public SqlConnectionString SqlConnectionString { get; }

    public ISqlServerConn CreateConnection()
    {
        return new SqlServerConn(SqlConnectionString, _logger);
    }
}