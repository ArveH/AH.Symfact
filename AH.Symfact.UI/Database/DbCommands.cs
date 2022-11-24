using AH.Symfact.UI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Data;
using System.Text;
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

    public Task<List<string>> GetAllTablesAsync()
    {
        return GetAllObjectsAsync("U");
    }

    public Task<List<string>> GetAllFunctionsAsync()
    {
        return GetAllObjectsAsync("FN");
    }

    public Task<List<string>> GetAllSchemaCollectionsAsync()
    {
        var sqlTxt = "select name from sys.xml_schema_collections where name != 'sys' order by name";
        return GetAllAsync(sqlTxt);
    }

    public async Task DeleteTablesAsync(IEnumerable<string> tableNames)
    {
        foreach (var tableName in tableNames)
        {
            await DeleteTableAsync(tableName);
        }
    }

    public async Task DeleteFunctionsAsync(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            await DeleteFunctionAsync(name);
        }
    }

    public async Task DeleteSchemaCollectionsAsync(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            await DeleteSchemaCollectionAsync(name);
        }
    }

    public async Task<int> InsertRowsAsync(string tableName, IEnumerable<TableRow>? input)
    {
        if (input == null) return 0;

        var sqlTxt = $"insert into {tableName} (DocName, Data) values(@DocName, @Xml)";
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        await using var cmd = new SqlCommand(sqlTxt, dbConn.Conn);
        cmd.Parameters.AddWithValue("@DocName", SqlDbType.NVarChar);
        cmd.Parameters.AddWithValue("@Xml", SqlDbType.Xml);
        var cnt = 0;
        foreach (var row in input)
        {
            cmd.Parameters[0].Value = row.DocName;
            cmd.Parameters[1].Value = row.Data.ToString();
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

    public async Task<bool> SchemaCollectionExistsAsync(string name)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        var sql =
            $"SELECT name FROM sys.xml_schema_collections WHERE name = '{name}'";
        await using var cmd = new SqlCommand(sql, dbConn.Conn);
        var res = await cmd.ExecuteScalarAsync() as string;
        return !string.IsNullOrWhiteSpace(res);
    }

    public Task DropSchemaCollectionAsync(string collectionName)
    {
        return ExecuteNonQuery($"DROP XML SCHEMA COLLECTION {collectionName}");
    }

    public Task CreateCollectionAsync(string collectionName, string xmlString)
    {
        return CreateOrAddToCollectionAsync(
            $"CREATE XML SCHEMA COLLECTION {collectionName} AS", 
            xmlString);
    }

    public Task AddToCollectionAsync(string collectionName, string xmlString)
    {
        return CreateOrAddToCollectionAsync(
            $"ALTER XML SCHEMA COLLECTION {collectionName} ADD", 
            xmlString);
    }

    public async Task CreateOrAddToCollectionAsync(string command, string xmlString)
    {
        var sb = new StringBuilder();
        sb.Append(command);
        sb.Append(" '");
        sb.Append(xmlString);
        sb.Append("'");
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        await using var cmd = new SqlCommand(sb.ToString(), dbConn.Conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private Task<List<string>> GetAllObjectsAsync(string type)
    {
        var sqlTxt = $"select name from sys.objects where type = '{type}' order by name";
        return GetAllAsync(sqlTxt);
    }

    private  async Task<List<string>> GetAllAsync(string sqlTxt)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
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

    private async Task ExecuteNonQuery(string sql)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        await using var cmd = new SqlCommand(sql, dbConn.Conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private Task DeleteTableAsync(string name)
    {
        return ExecuteNonQuery($"drop table {name}");
    }

    private Task DeleteFunctionAsync(string name)
    {
        return ExecuteNonQuery($"drop function {name}");
    }

    private Task DeleteSchemaCollectionAsync(string name)
    {
        return ExecuteNonQuery($"drop xml schema collection {name}");
    }
}