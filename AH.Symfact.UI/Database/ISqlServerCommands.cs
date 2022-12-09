using System.Collections.Generic;
using System.Threading.Tasks;
using AH.Symfact.UI.Models;

namespace AH.Symfact.UI.Database;

public interface ISqlServerCommands
{
    Task<List<string>> GetAllTablesAsync();
    Task DeleteTablesAsync(IEnumerable<string> tableNames);
    Task<int> InsertRowsAsync(string tableName, IEnumerable<TableRow>? input);
    Task<bool> FullTextIndexExistsAsync(string tableName);
    Task DropFullTextIndexAsync(string tableName);
    Task CreateFullTextIndexAsync(string tableName, string catalogName);
    Task<List<string>> GetAllSchemaCollectionsAsync();
    Task<bool> SchemaCollectionExistsAsync(string name);
    Task DeleteSchemaCollectionsAsync(IEnumerable<string> names);
    Task DropSchemaCollectionAsync(string collectionName);
    Task CreateCollectionAsync(string collectionName, string xmlString);
    Task AddToCollectionAsync(string collectionName, string xmlString);
    Task CreateOrAddToCollectionAsync(string command, string xmlString);
    Task<List<string>> GetAllFunctionsAsync();
    Task DeleteFunctionsAsync(IEnumerable<string> names);
    Task ExecuteScriptAsync(string script);
    Task<List<string>> GetAllFullTextIndexesAsync();
    Task CreateFulltextCatalogAsync(string catalogName);
    int ExecuteQuery(string script);
}