using Microsoft.Data.SqlClient;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AH.Symfact.UI.Database;

public class DbCommands : IDbCommands
{
    private readonly IDbConnFactory _dbConnFactory;
    private readonly ILogger _logger;

    public DbCommands(
        IDbConnFactory dbConnFactory,
        ILogger logger)
    {
        _dbConnFactory = dbConnFactory;
        _logger = logger.ForContext<DbCommands>();
    }

    public async Task<IEnumerable<string>> GetAllTablesAsync()
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

    public Task DeleteAllTablesAsync()
    {
        throw new System.NotImplementedException();
    }
}