namespace AH.Symfact.UI.SqlServer;

public interface IDbConnFactory
{
    SqlConnectionString SqlConnectionString { get; }
    ISqlServerConn CreateConnection();
}