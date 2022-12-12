namespace AH.Symfact.UI.SqlServer;

public interface ISqlConnectionFactory
{
    SqlConnectionString SqlConnectionString { get; }
    ISqlServerConn CreateConnection();
}