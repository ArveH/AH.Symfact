namespace AH.Symfact.UI.Database;

public interface IDbConnFactory
{
    SqlConnectionString SqlConnectionString { get; }
    ISqlServerConn CreateConnection();
}