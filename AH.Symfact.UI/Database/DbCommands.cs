using AH.Symfact.UI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AH.Symfact.UI.Database;

public class DbCommands : IDbCommands
{
    private readonly IDbConnFactory _dbConnFactory;

    public DbCommands(
        IDbConnFactory dbConnFactory)
    {
        _dbConnFactory = dbConnFactory;
    }

    public async Task<List<string>> GetAllTablesAsync()
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        var sqlTxt = "select name from sys.objects where type = 'U' order by name";
        await using var cmd = new SqlCommand(sqlTxt, dbConn.Conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        var tables = new List<string>();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        await reader.CloseAsync();
        return tables;
    }

    public async Task DeleteTablesAsync(IEnumerable<string> tableNames)
    {
        foreach (var tableName in tableNames)
        {
            await DeleteTableAsync(tableName);
        }
    }

    public async Task<int> InsertRowsAsync(string tableName, IEnumerable<TableRow>? input)
    {
        if (input == null) return 0;

        var sqlTxt = $"insert into {tableName} (Data) values(@Xml)";
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        await using var cmd = new SqlCommand(sqlTxt, dbConn.Conn);
        cmd.Parameters.AddWithValue("@Xml", SqlDbType.Xml);
        var cnt = 0;
        foreach (var row in input)
        {
            cmd.Parameters[0].Value = row.Data;
            await cmd.ExecuteNonQueryAsync();
            cnt++;
        }

        return cnt;
    }

    public async Task ExecuteScriptAsync(string script)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        var server = new Server(new ServerConnection(dbConn.Conn));
        var _ = server.ConnectionContext.ExecuteNonQuery(script);
    }

    private async Task DeleteTableAsync(string tableName)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        var sqlTxt = $"drop table {tableName}";
        await using var cmd = new SqlCommand(sqlTxt, dbConn.Conn);
        await cmd.ExecuteNonQueryAsync();
    }
}