using System;
using System.Data;
using System.Threading.Tasks;
using AH.Symfact.UI.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace AH.Symfact.UI.Database;

public class DbConn : IDbConn
{
    private readonly DbConnectionString _dbConnectionString;
    private readonly ILogger _logger;

    public DbConn(
        DbConnectionString dbConnectionString,
        ILogger logger)
    {
        _dbConnectionString = dbConnectionString;
        _logger = logger.ForContext<DbConn>();
    }

    public string? DbName => Conn?.Database;
    public SqlConnection? Conn { get; private set; }
    public bool IsConnected => Conn is { State: ConnectionState.Open };

    public async Task<bool> ConnectAsync()
    {
        if (IsConnected) return true;

        try
        {
            Conn = new SqlConnection(_dbConnectionString.ConnectionString);
            await Conn.OpenAsync();
            var sqlTxt = "select @@VERSION";

            await using var cmd = new SqlCommand(sqlTxt, Conn);
            var version = (string?)await cmd.ExecuteScalarAsync();
            if (!string.IsNullOrWhiteSpace(version))
            {
                _logger.Debug("Connected to '{DbName}'. Database version: {DbVersion}", 
                    Conn.Database, version);
                return true;
            }

            _logger.Error("Connect failed!");
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.FlattenMessages());
            if (Conn != null) await Conn.CloseAsync();
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (Conn != null)
            await Conn.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}