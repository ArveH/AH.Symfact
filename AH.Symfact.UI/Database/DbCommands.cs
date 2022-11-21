using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace AH.Symfact.UI.Database;

public class DbCommands : IDbCommands
{
    private readonly ILogger _logger;

    public DbCommands(ILogger logger)
    {
        _logger = logger.ForContext<DbCommands>();
    }

    public Task<IEnumerable<string>> GetAllTables()
    {

        throw new System.NotImplementedException();
    }

    public Task DeleteAllTables()
    {
        throw new System.NotImplementedException();
    }
}