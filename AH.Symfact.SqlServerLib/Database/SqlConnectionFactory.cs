namespace AH.Symfact.SqlServerLib.Database;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly ILogger _logger;

    public SqlConnectionFactory(
        SqlConnectionString sqlConnectionString,
        ILogger logger)
    {
        SqlConnectionString = sqlConnectionString;
        _logger = logger.ForContext<SqlConnectionFactory>();
    }

    public SqlConnectionString SqlConnectionString { get; }

    public ISqlServerConn CreateConnection()
    {
        return new SqlServerConn(SqlConnectionString, _logger);
    }
}