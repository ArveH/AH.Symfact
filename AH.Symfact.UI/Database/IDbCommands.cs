using System.Collections.Generic;
using System.Threading.Tasks;

namespace AH.Symfact.UI.Database;

public interface IDbCommands
{
    Task<IEnumerable<string>> GetAllTablesAsync();
    Task DeleteAllTablesAsync();
}