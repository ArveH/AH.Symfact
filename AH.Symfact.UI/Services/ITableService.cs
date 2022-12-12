namespace AH.Symfact.UI.Services;

public interface ITableService
{
    Task CreateTableAsync(
        string tableName,
        string scriptFile,
        string xmlDataFile);
    Task ExecuteScriptAsync(
        string tableName, string fileName);

    Task CreateFullIndexesAsync();
}