using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace AH.Symfact.UI.Database;

public interface IDbConn : IAsyncDisposable
{
    string? DbName { get; }
    SqlConnection? Conn { get; }
    bool IsConnected { get; }

    Task<bool> ConnectAsync();
    Task DisconnectAsync();
}