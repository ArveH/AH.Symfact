using AH.Symfact.UI.Extensions;
using Microsoft.Data.SqlClient;
using Serilog;
using System;
using System.Data;
using System.Threading.Tasks;

namespace AH.Symfact.UI.Database;

public class SqlServerConn : ISqlServerConn
{
    private readonly SqlConnectionString _sqlConnectionString;
    private readonly ILogger _logger;

    public SqlServerConn(
        SqlConnectionString sqlConnectionString,
        ILogger logger)
    {
        _sqlConnectionString = sqlConnectionString;
        _logger = logger.ForContext<SqlServerConn>();
    }

    public string? DbName => Conn?.Database;
    public SqlConnection? Conn { get; private set; }
    public bool IsConnected => Conn is { State: ConnectionState.Open };

    public async Task<bool> ConnectAsync()
    {
        if (IsConnected) return true;

        try
        {
            Conn = new SqlConnection(_sqlConnectionString.ConnectionString);
            await Conn.OpenAsync();
            var sqlTxt = "select @@VERSION";

            await using var cmd = new SqlCommand(sqlTxt, Conn);
            var version = (string?)await cmd.ExecuteScalarAsync();
            if (!string.IsNullOrWhiteSpace(version))
            {
                _logger.Verbose("Connected to '{DbName}'. Database version: {DbVersion}",
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

    public bool Connect()
    {
        if (IsConnected) return true;

        try
        {
            Conn = new SqlConnection(_sqlConnectionString.ConnectionString);
            Conn.Open();
            var sqlTxt = "select @@VERSION";

            using var cmd = new SqlCommand(sqlTxt, Conn);
            var version = (string?)cmd.ExecuteScalar();
            if (!string.IsNullOrWhiteSpace(version))
            {
                _logger.Verbose("Connected to '{DbName}'. Database version: {DbVersion}",
                    Conn.Database, version);
                return true;
            }

            _logger.Error("Connect failed!");
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.FlattenMessages());
            if (Conn != null) Conn.Close();
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (Conn != null)
        {
            await Conn.DisposeAsync();
            Conn = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    public void Dispose()
    {
        Conn?.Dispose();
        Conn = null;
    }
}