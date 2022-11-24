using System.Collections.Generic;
using System.Threading.Tasks;
using AH.Symfact.UI.Models;

namespace AH.Symfact.UI.Database;

public interface IDbCommands
{
    Task<List<string>> GetAllTablesAsync();
    Task<List<string>> GetAllFunctionsAsync();
    Task<List<string>> GetAllSchemaCollectionsAsync();
    Task DeleteTablesAsync(IEnumerable<string> tableNames);
    Task DeleteFunctionsAsync(IEnumerable<string> names);
    Task DeleteSchemaCollectionsAsync(IEnumerable<string> names);
    Task ExecuteScriptAsync(string script);
    Task<int> InsertRowsAsync(string tableName, IEnumerable<TableRow>? input);
    Task<bool> SchemaCollectionExistsAsync(string name);
    Task DropSchemaCollectionAsync(string collectionName);
    Task CreateCollectionAsync(string collectionName, string xmlString);
    Task AddToCollectionAsync(string collectionName, string xmlString);
}