using System;
using System.IO;
using System.Threading.Tasks;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;

namespace AH.Symfact.UI.Services;

public class SchemaService : ISchemaService
{
    private readonly IDbCommands _dbCommands;
    private readonly ILogger _logger;

    public SchemaService(
        IDbCommands dbCommands,
        ILogger logger)
    {
        _dbCommands = dbCommands;
        _logger = logger.ForContext<SchemaService>();
    }

    public async Task<bool> CreateCollectionAsync(string collectionName, string fileName)
    {
        try
        {
            var xmlString = await GetXmlStringAsync(fileName);

            if (await _dbCommands.SchemaCollectionExistsAsync(collectionName))
            {
                await _dbCommands.DropSchemaCollectionAsync(collectionName);
                _logger.Debug("Schema collection '{SchemaCollectionName}' dropped", collectionName);
            }

            await _dbCommands.CreateCollectionAsync(collectionName, xmlString);
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

            await _dbCommands.AddToCollectionAsync(collectionName, xmlString);
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