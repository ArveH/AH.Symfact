namespace AH.Symfact.SqlServerLib.Database;

public interface ISqlConnectionFactory
{
    SqlConnectionString SqlConnectionString { get; }
    ISqlServerConn CreateConnection();
}