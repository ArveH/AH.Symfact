using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AH.Symfact.UI.Controls;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Windows.Input;
using System.Xml.Linq;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Models;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AH.Symfact.UI.ViewModels;

public partial class CreateTablesViewModel : ObservableRecipient
{
    private readonly IDbCommands _dbCommands;
    private readonly ITaminoFileReader _fileReader;
    private readonly ILogger _logger;

    public CreateTablesViewModel(
        IDbCommands dbCommands,
        ITaminoFileReader fileReader,
        ILogger logger)
    {
        _dbCommands = dbCommands;
        _fileReader = fileReader;
        _logger = logger.ForContext<MenuControl>();
        NoColumnsCommand = new AsyncRelayCommand(CreateWithNoColumns);
        ComputedColumnsCommand = new AsyncRelayCommand(CreateWithComputedColumns);
        ExtractedColumnsCommand = new AsyncRelayCommand(CreateWithExtractedColumns);
        WeakReferenceMessenger.Default.Register<ContractChangedMessage>(this, (r, msg) =>
        {
            switch (msg.Value.Action)
            {
                case TableAction.LoadedXml:
                    Heading = $"Contract tables {msg.Value.Message} rows";
                    break;
                case TableAction.NoColumns:
                    NoColumnsStatus = msg.Value.Message ?? "<Message missing>";
                    break;
                case TableAction.ComputedColumns:
                    ComputedColumnsStatus = msg.Value.Message ?? "<Message missing>";
                    break;
                case TableAction.ExtractedColumns:
                    ExtractedColumnsStatus = msg.Value.Message ?? "<Message missing>";
                    break;
            }
        });
    }

    [ObservableProperty]
    private string _heading = "Contract tables";
    [ObservableProperty]
    private string _noColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _computedColumnsStatus = "Ready...";
    [ObservableProperty]
    private string _extractedColumnsStatus = "Ready...";

    public ICommand NoColumnsCommand { get; }
    public ICommand ComputedColumnsCommand { get; }
    public ICommand ExtractedColumnsCommand { get; }

    private async Task CreateWithNoColumns()
    {
        await CreateTableAsync("Contract",
            cnt => WeakReferenceMessenger.Default.Send(new ContractLoadedMessage(cnt)));
    }

    private async Task CreateWithComputedColumns()
    {
        await CreateTableAsync("Contract",
            cnt => WeakReferenceMessenger.Default.Send(new ContractLoadedMessage(cnt)));
    }

    private Task CreateWithExtractedColumns()
    {
        throw new System.NotImplementedException();
    }

    private async Task CreateTableAsync(string entityName, Func<int, ValueChangedMessage<int>> func)
    {
        try
        {
            await ExecuteScriptAsync(entityName + ".sql", s => NoColumnsStatus = s);
            var contractData = ReadFromXml(entityName);

            func(0);
            _logger.Information("Inserting into {TableName}...", entityName);
            NoColumnsStatus = $"Inserting into {entityName}...";
            var cnt = await _dbCommands.InsertRowsAsync(entityName, contractData);
            _logger.Information("{RowCount} rows inserted into {TableName}", cnt, entityName);
            NoColumnsStatus = $"{cnt} rows inserted into {entityName}";
            func(cnt);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Insert into {TableName} failed", entityName);
            NoColumnsStatus = $"Insert into {entityName} failed. " + ex.FlattenMessages();
        }
    }

    private async Task ExecuteScriptAsync(
        string fileName, Action<string> writeLog)
    {
        try
        {
            _logger.Information("Running script '{FileName}'...", fileName);
            writeLog($"Running script '{fileName}'...");
            var folder = WeakReferenceMessenger.Default.Send<ExeFolderMessage>();
            var filePath = Path.Combine(folder, "Database", "Scripts", fileName);
            var scriptTxt = await File.ReadAllTextAsync(filePath);
            var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
            await _dbCommands.ExecuteScriptAsync(
                scriptTxt.Replace("<<<PATH>>>", dataPath));
            _logger.Information("Script '{FileName}' finished", fileName);
            writeLog($"Script '{fileName}' finished");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed running script '{FileName}'", fileName);
            writeLog($"FAILED running script '{fileName}'! " + ex.FlattenMessages());
        }
    }

    private IEnumerable<TableRow>? ReadFromXml(string entityName)
    {
        var dataPath = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var filePath = Path.Combine(dataPath, $"{entityName}.xml");
        _logger.Information("Starting to load from file '{FilePath}'...",
            filePath);
        NoColumnsStatus = $"Starting to load from file '{filePath}'...";
        try
        {
            var xElem = XElement.Load(filePath);
            var elemName = _fileReader.GetName(xElem);
            if (elemName == null)
            {
                _logger.Error("Can't find element name for {EntityName}", entityName);
                NoColumnsStatus = $"Can't find element name for {entityName}";
                return null;
            }

            var xmlData = _fileReader.SplitRequests(xElem);
            _logger.Information("Found {RowCount} rows for '{ElementName}'",
                xmlData.Count, elemName);
            NoColumnsStatus = $"Found {xmlData.Count} rows for '{elemName}'";
            return xmlData;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Can't read XML file for {EntityName}", entityName);
            NoColumnsStatus = $"Can't read XML file for {entityName}";
            return null;
        }
    }
}