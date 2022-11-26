using AH.Symfact.UI.Database;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using AH.Symfact.UI.Extensions;
using CommunityToolkit.Mvvm.Input;

namespace AH.Symfact.UI.ViewModels;

public partial class TestingViewModel : ObservableRecipient
{
    private readonly ISchemaService _schemaService;
    private readonly ITableService _tableService;
    private readonly IDbCommands _dbCommands;
    private readonly ILogger _logger;

    public TestingViewModel
    (
        ISchemaService schemaService,
        ITableService tableService,
        IDbCommands dbCommands,
        ILogger logger)
    {
        _schemaService = schemaService;
        _tableService = tableService;
        _dbCommands = dbCommands;
        _logger = logger.ForContext<TestingViewModel>();
        ExecuteSequentialCommand = new AsyncRelayCommand(ExecuteSequentialAsync);
        ExecuteParallelCommand = new AsyncRelayCommand(ExecuteParallelAsync);
        ClearMessagesCommand = new RelayCommand(ClearMessages);
    }

    [ObservableProperty]
    private string _selectedFile = string.Empty;
    [ObservableProperty]
    private ObservableCollection<string> _queryFiles = new();
    [ObservableProperty]
    private ObservableCollection<string> _messages = new();
    [ObservableProperty]
    private int _sequentialCount = 10;
    partial void OnSequentialCountChanging(int value)
    {
        Messages.Add($"SequentialCount changed to {value}");
    }

    [ObservableProperty]
    private int _parallelCount = 10;
    partial void OnParallelCountChanging(int value)
    {
        Messages.Add($"ParallelCount changed to {value}");
    }

    public void QueryFileChanged(string? queryFile)
    {
        if (!string.IsNullOrWhiteSpace(queryFile))
        {
            SelectedFile = queryFile;
            Messages.Add($"Script file = '{SelectedFile}'");
        }
    }

    public IAsyncRelayCommand ExecuteSequentialCommand { get; }
    public IAsyncRelayCommand ExecuteParallelCommand { get; }
    public IRelayCommand ClearMessagesCommand { get; }

    private void ClearMessages()
    {
        Messages.Clear();
    }

    private async Task<bool> ExecuteScriptAsync(int index, int total)
    {
        try
        {
            var script = await File.ReadAllTextAsync(SelectedFile);
            await _dbCommands.ExecuteScriptAsync(script);
            _logger.Information("({Index} of {Total}) Script '{FileName}' executed", 
                index, total, SelectedFile);
            Messages.Add($"({index} of {total}) Script '{SelectedFile}' executed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Executing script '{FileName}' ({Index} of {Total}) failed", 
                SelectedFile, index, total);
            Messages.Add($"Executing script Script '{SelectedFile}' ({index} of {total}) failed. " + ex.FlattenMessages());
            return false;
        }
    }

    private async Task ExecuteSequentialAsync()
    {
        for (var i = 0; i < SequentialCount; i++)
        {
            await ExecuteScriptAsync(i+1, SequentialCount);
        }
    }

    private async Task ExecuteParallelAsync()
    {
        var tasks = new List<Task>();
        for (var i = 0; i < ParallelCount; i++)
        {
            tasks.Add(ExecuteScriptAsync(i+1, ParallelCount));
        }

        await Task.WhenAll(tasks);
    }

    public void SelectQueryFile()
    {
        try
        {
            var queryFolder = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>() + @"\Queries";
            _logger.Debug("Getting files from {QueryFolder}...", queryFolder);
            var files = Directory.GetFiles(queryFolder, "*.sql");
            if (files.Length < 1)
            {
                _logger.Information("No files found in folder: '{QueryFolder}'", queryFolder);
                return;
            }

            SelectedFile = string.Empty;
            QueryFiles.Clear();
            foreach (var file in files)
            {
                QueryFiles.Add(file);
            }
            _logger.Debug("{FileCount} files found in folder '{QueryFolder}'",
                files.Length, queryFolder);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to open file picker for XSD files");
        }
    }
}