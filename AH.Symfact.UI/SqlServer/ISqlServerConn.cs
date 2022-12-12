namespace AH.Symfact.UI.SqlServer;

public interface ISqlServerConn : IAsyncDisposable, IDisposable
{
    string? DbName { get; }
    SqlConnection? Conn { get; }
    bool IsConnected { get; }

    Task<bool> ConnectAsync();
    Task DisconnectAsync();
    bool Connect();
}