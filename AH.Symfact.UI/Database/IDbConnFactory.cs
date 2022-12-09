namespace AH.Symfact.UI.Database;

public interface IDbConnFactory
{
    SqlConnectionString SqlConnectionString { get; }
    IDbConn CreateConnection();
}