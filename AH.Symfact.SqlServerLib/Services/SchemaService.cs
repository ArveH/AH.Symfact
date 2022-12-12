namespace AH.Symfact.SqlServerLib.Services;

public class SchemaService : ISchemaService
{
    private readonly ISqlServerCommands _sqlServerCommands;
    private readonly ILogger _logger;

    public SchemaService(
        ISqlServerCommands sqlServerCommands,
        ILogger logger)
    {
        _sqlServerCommands = sqlServerCommands;
        _logger = logger.ForContext<SchemaService>();
    }

    public async Task<bool> CreateCollectionAsync(string collectionName, string fileName)
    {
        try
        {
            var xmlString = await GetXmlStringAsync(fileName);

            if (await _sqlServerCommands.SchemaCollectionExistsAsync(collectionName))
            {
                await _sqlServerCommands.DropSchemaCollectionAsync(collectionName);
                _logger.Debug("Schema collection '{SchemaCollectionName}' dropped", collectionName);
            }

            await _sqlServerCommands.CreateCollectionAsync(collectionName, xmlString);
            _logger.Debug("Schema collection '{SchemaCollectionName}' created from '{FileName}'", 
                collectionName, fileName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't create schema collection '{SchemaCollectionName}' from '{FileName}'.", 
                collectionName, fileName);
            return false;
        }
    }

    public async Task<bool> AddToCollectionAsync(string collectionName, string fileName)
    {
        try
        {
            var xmlString = await GetXmlStringAsync(fileName);

            await _sqlServerCommands.AddToCollectionAsync(collectionName, xmlString);
            _logger.Debug("Added '{FileName}' to collection '{SchemaCollectionName}' created", 
                fileName, collectionName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't add '{FileName}' to schema collection '{SchemaCollectionName}'.", 
                fileName, collectionName);
            return false;
        }
    }

    private static async Task<string> GetXmlStringAsync(string fileName)
    {
        var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var filePath = Path.Combine(dataPath, "Schemas", fileName);
        var xmlString = await File.ReadAllTextAsync(filePath);
        return xmlString.Replace("'", "''");
    }
}