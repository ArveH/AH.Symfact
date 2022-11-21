using System.Collections.Generic;
using System.Threading.Tasks;

namespace AH.Symfact.UI.Database;

public interface IDbCommands
{
    Task<List<string>> GetAllTablesAsync();
    Task DeleteTablesAsync(IEnumerable<string> tableNames);
}