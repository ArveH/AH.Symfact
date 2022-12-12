namespace AH.Symfact.SqlServerLib.Services;

public class TableService : ITableService
{
    private readonly ISqlServerCommands _sqlServerCommands;
    private readonly ITaminoFileReader _fileReader;
    private readonly ILogger _logger;

    public TableService(
        ISqlServerCommands sqlServerCommands,
        ITaminoFileReader fileReader,
        ILogger logger)
    {
        _sqlServerCommands = sqlServerCommands;
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

            var cnt = await _sqlServerCommands.InsertRowsAsync(tableName, contractData);

            _logger.Information("{RowCount} rows inserted into {TableName}", cnt, tableName);
            Send(tableName, TableAction.LoadedTable, $"{cnt} rows inserted into {tableName}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Insert into {TableName} failed", tableName);
            SendInfo(tableName, $"Insert into {tableName} failed. " + ex.FlattenMessages());
        }
    }

    public async Task ExecuteScriptAsync(
        string tableName, string fileName)
    {
        try
        {
            _logger.Information("Running script '{FileName}'...", fileName);
            SendInfo(tableName, $"Running script '{fileName}'...");
            var folder = WeakReferenceMessenger.Default.Send<ExeFolderMessage>();
            var filePath = Path.Combine(folder, "Database", "Scripts", fileName);
            var scriptTxt = await File.ReadAllTextAsync(filePath);
            await _sqlServerCommands.ExecuteScriptAsync(scriptTxt);
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

    public async Task CreateFullIndexesAsync()
    {
        await DropAllFullTextIndexesAsync();
        await CreateAllFullTextCatalogsAsync();
        await CreateAllFullTextIndexesAsync();
    }

    private async Task CreateAllFullTextCatalogsAsync()
    {
        await _sqlServerCommands.CreateFulltextCatalogAsync(SymfactConstants.Name.ContractCatalog);
        await _sqlServerCommands.CreateFulltextCatalogAsync(SymfactConstants.Name.OrgPersonCatalog);
        await _sqlServerCommands.CreateFulltextCatalogAsync(SymfactConstants.Name.PartyCatalog);
    }

    private async Task CreateAllFullTextIndexesAsync()
    {
        foreach (var tableName in SymfactConstants.AllTables)
        {
            await CreateFullTextIndexAsync(tableName, tableName.CatalogName());
        }
    }

    private async Task DropAllFullTextIndexesAsync()
    {
        var indexes = await _sqlServerCommands.GetAllFullTextIndexesAsync();
        foreach (var index in indexes)
        {
            await _sqlServerCommands.DropFullTextIndexAsync(index);
        }
    }

    private async Task CreateFullTextIndexAsync(string tableName, string catalogName)
    {
        try
        {
            _logger.Information("Creating full-text index on '{TableName}'...", tableName);
            SendInfo(tableName, $"Creating full-text index on '{tableName}'...");
            if (await _sqlServerCommands.FullTextIndexExistsAsync(tableName))
            {
                await _sqlServerCommands.DropFullTextIndexAsync(tableName);
            }
            await _sqlServerCommands.CreateFullTextIndexAsync(tableName, catalogName);
            _logger.Information("Full-text index created on '{TableName}'", tableName);
            SendInfo(tableName, $"Full-text index created on '{tableName}'");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "FAILED to create full-text index on '{TableName}'", tableName);
            SendInfo(
                tableName,
                $"FAILED to create full-text index on '{tableName}'! " + ex.FlattenMessages());
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