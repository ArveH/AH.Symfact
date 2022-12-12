using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using AH.Symfact.UI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace AH.Symfact.UI.SqlServer;

public class SqlServerCommands : ISqlServerCommands
{
    private readonly IDbConnFactory _dbConnFactory;

    public SqlServerCommands(
        IDbConnFactory dbConnFactory)
    {
        _dbConnFactory = dbConnFactory;
    }

    #region Table
    public Task<List<string>> GetAllTablesAsync()
    {
        return GetAllObjectsAsync("U");
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

    private Task DeleteTableAsync(string name)
    {
        return ExecuteNonQuery($"drop table {name}");
    }
    #endregion

    #region Full-text index
    public Task<List<string>> GetAllFullTextIndexesAsync()
    {
        var sqlTxt = "select object_name(object_id) AS TableName from sys.fulltext_indexes";
        return GetAllAsync(sqlTxt);
    }

    public Task<bool> FullTextIndexExistsAsync(string tableName)
    {
        var sql =
            $"SELECT object_id FROM sys.fulltext_indexes WHERE object_name(object_id) = '{tableName}'";
        return ExistsAsync(sql);
    }

    public Task DropFullTextIndexAsync(string tableName)
    {
        return ExecuteNonQuery($"DROP FULLTEXT INDEX ON {tableName}");
    }

    public Task CreateFullTextIndexAsync(string tableName, string catalogName)
    {
        return ExecuteNonQuery(
            $"CREATE FULLTEXT INDEX ON {tableName} (Data)\r\n" +
            $"    KEY INDEX PK_{tableName}_Id \r\n" +
            $"        ON {catalogName};");
    }

    public async Task CreateFulltextCatalogAsync(string catalogName)
    {
        var sql =
            $"SELECT name FROM sys.fulltext_catalogs WHERE name = '{catalogName}'";
        if (await ExistsAsync(sql))
        {
            await ExecuteNonQuery($"DROP FULLTEXT CATALOG {catalogName}");
        }
        await ExecuteNonQuery($"CREATE FULLTEXT CATALOG {catalogName}");
    }
    #endregion

    #region Schema Collection
    public Task<List<string>> GetAllSchemaCollectionsAsync()
    {
        var sqlTxt = "select name from sys.xml_schema_collections where name != 'sys' order by name";
        return GetAllAsync(sqlTxt);
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

    public async Task DeleteSchemaCollectionsAsync(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            await DropSchemaCollectionAsync(name);
        }
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
    #endregion

    #region Function
    public Task<List<string>> GetAllFunctionsAsync()
    {
        return GetAllObjectsAsync("FN");
    }

    public async Task DeleteFunctionsAsync(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            await DeleteFunctionAsync(name);
        }
    }

    private Task DeleteFunctionAsync(string name)
    {
        return ExecuteNonQuery($"drop function {name}");
    }
    #endregion

    public async Task ExecuteScriptAsync(string script)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        var server = new Server(new ServerConnection(dbConn.Conn));
        var _ = server.ConnectionContext.ExecuteNonQuery(script);
    }

    public int ExecuteQuery(string script)
    {
        using var dbConn = _dbConnFactory.CreateConnection();
        dbConn.Connect();
        var server = new Server(new ServerConnection(dbConn.Conn));
        var result = server.ConnectionContext.ExecuteWithResults(script);
        return result.Tables[0].Rows.Count;
    }

    private Task<List<string>> GetAllObjectsAsync(string type)
    {
        var sqlTxt = $"select name from sys.objects where type = '{type}' order by name";
        return GetAllAsync(sqlTxt);
    }

    private async Task<List<string>> GetAllAsync(string sqlTxt)
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

    public async Task<bool> ExistsAsync(string sql)
    {
        await using var dbConn = _dbConnFactory.CreateConnection();
        await dbConn.ConnectAsync();
        await using var cmd = new SqlCommand(sql, dbConn.Conn);
        var res = await cmd.ExecuteScalarAsync() as string;
        return !string.IsNullOrWhiteSpace(res);
    }
}