namespace AH.Symfact.UI.Database;

public interface IDbConnFactory
{
    DbConnectionString DbConnectionString { get; }
    IDbConn CreateConnection();
}