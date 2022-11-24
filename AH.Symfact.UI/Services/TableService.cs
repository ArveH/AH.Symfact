using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Models;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AH.Symfact.UI.Services;

public class TableService : ITableService
{
    private readonly IDbCommands _dbCommands;
    private readonly ITaminoFileReader _fileReader;
    private readonly ILogger _logger;

    public TableService(
        IDbCommands dbCommands,
        ITaminoFileReader fileReader,
        ILogger logger)
    {
        _dbCommands = dbCommands;
        _fileReader = fileReader;
        _logger = logger;
    }

    public async Task CreateTableAsync(
        string tableName,
        string scriptFile,
        string xmlDataFile)
    {
        try
        {
            await ExecuteScriptAsync(tableName, scriptFile);
            var contractData = ReadFromXml(tableName, xmlDataFile);

            _logger.Information("Inserting into {TableName}...", tableName);
            SendInfo(tableName, $"Inserting into {tableName}...");

            var cnt = await _dbCommands.InsertRowsAsync(tableName, contractData);

            _logger.Information("{RowCount} rows inserted into {TableName}", cnt, tableName);
            Send(tableName, TableAction.LoadedTable, $"{cnt} rows inserted into {tableName}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Insert into {TableName} failed", tableName);
            SendInfo(tableName, $"Insert into {tableName} failed. " + ex.FlattenMessages());
        }
    }

    private IEnumerable<TableRow>? ReadFromXml(
        string tableName, string xmlDataFile)
    {
        var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var filePath = Path.Combine(dataPath, xmlDataFile);
        _logger.Information("Starting to load from file '{FilePath}'...",
            filePath);
        SendInfo(tableName, $"Starting to load from file '{filePath}'...");
        try
        {
            var xElem = XElement.Load(filePath);
            var elemName = _fileReader.GetName(xElem);
            if (elemName == null)
            {
                _logger.Error("Can't find element name for {EntityName}", tableName);
                SendInfo(tableName, $"Can't find element name for {tableName}");
                return null;
            }

            var xmlData = _fileReader.SplitRequests(xElem);
            _logger.Information("Found {RowCount} rows for '{ElementName}'",
                xmlData.Count, elemName);
            Send(tableName, TableAction.LoadedXml, $"Found {xmlData.Count} rows for '{elemName}'", xmlData.Count);
            return xmlData;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't read XML file for {EntityName}", tableName);
            SendInfo(tableName, $"Can't read XML file for {tableName}");
            return null;
        }
    }

    private async Task ExecuteScriptAsync(
        string tableName, string fileName)
    {
        try
        {
            _logger.Information("Running script '{FileName}'...", fileName);
            SendInfo(tableName, $"Running script '{fileName}'...");
            var folder = WeakReferenceMessenger.Default.Send<ExeFolderMessage>();
            var filePath = Path.Combine(folder, "Database", "Scripts", fileName);
            var scriptTxt = await File.ReadAllTextAsync(filePath);
            await _dbCommands.ExecuteScriptAsync(scriptTxt);
            _logger.Information("Script '{FileName}' finished", fileName);
            SendInfo(tableName, $"Script '{fileName}' finished");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed running script '{FileName}'", fileName);
            SendInfo(
                tableName,
                $"FAILED running script '{fileName}'! " + ex.FlattenMessages());
        }
    }

    private static void SendInfo(string tableName, string msg)
    {
        Send(tableName, TableAction.Information, msg);
    }

    private static void Send(string tableName, TableAction action, string msg, int rows = 0)
    {
        WeakReferenceMessenger.Default.Send(
            new TableChangedMessage(
                new TableChanged(tableName, action, msg) { Rows = rows }));
    }
}