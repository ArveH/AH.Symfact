using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Models;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace AH.Symfact.UI.ViewModels;

public partial class TestingViewModel : ObservableRecipient
{
    private readonly IDbCommands _dbCommands;
    private readonly ILogger _logger;

    public TestingViewModel
    (
        IDbCommands dbCommands,
        ILogger logger)
    {
        _dbCommands = dbCommands;
        _logger = logger.ForContext<TestingViewModel>();
        ExecuteSequentialCommand = new AsyncRelayCommand(ExecuteSequentialAsync);
        ExecuteParallelCommand = new AsyncRelayCommand(ExecuteParallelAsync);
        ClearMessagesCommand = new RelayCommand(ClearMessages);
        DispatcherQueue = WeakReferenceMessenger.Default.Send<DispatcherQueueMessage>();
    }

    public DispatcherQueue? DispatcherQueue { get; }

    [ObservableProperty]
    private string _selectedFile = string.Empty;
    [ObservableProperty]
    private string _tableType = SymfactConstants.TableTypes[0];
    [ObservableProperty]
    private List<string> _tableTypes = new(SymfactConstants.TableTypes);
    [ObservableProperty]
    private ObservableCollection<string> _queryFiles = new();
    [ObservableProperty]
    private ObservableCollection<string> _messages = new();
    [ObservableProperty]
    private int _sequentialCount = 10;
    partial void OnSequentialCountChanging(int value)
    {
        _logger.Information("SequentialCount changed to {SequentialCount}", value);
        WriteMessage($"SequentialCount changed to {value}");
    }

    [ObservableProperty]
    private int _parallelCount = 10;
    partial void OnParallelCountChanging(int value)
    {
        _logger.Information("ParallelCount changed to {ParallelCount}", value);
        WriteMessage($"ParallelCount changed to {value}");
    }

    public void QueryFileChanged(string? queryFile)
    {
        if (!string.IsNullOrWhiteSpace(queryFile))
        {
            SelectedFile = queryFile;
            _logger.Information("Script file = '{SelectedFile}'", SelectedFile);
            WriteMessage($"Script file = '{SelectedFile}'");
        }
    }

    public void TableTypeChanged()
    {

    }

    public IAsyncRelayCommand ExecuteSequentialCommand { get; }
    public IAsyncRelayCommand ExecuteParallelCommand { get; }
    public IRelayCommand ClearMessagesCommand { get; }

    private void ClearMessages()
    {
        Messages.Clear();
    }

    private async Task<ScriptResult> ExecuteScriptAsync(int index, int total)
    {
        try
        {
            var script = await GetScriptAsync();
            var sw = new Stopwatch();
            _logger.Debug("Executing Script '{FileName}' ({Index} of {Total}) ...",
                SelectedFile, index, total);
            sw.Start();
            await _dbCommands.ExecuteScriptAsync(script);
            sw.Stop();
            _logger.Information("({Index} of {Total}) Script '{FileName}' executed in {Ms}ms",
                index, total, SelectedFile, sw.ElapsedMilliseconds);
            WriteMessage($"({index} of {total}) Script '{SelectedFile}' executed in {sw.ElapsedMilliseconds}ms");
            return new ScriptResult(sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Executing script '{FileName}' ({Index} of {Total}) failed",
                SelectedFile, index, total);
            WriteMessage($"Executing script Script '{SelectedFile}' ({index} of {total}) failed. " + ex.FlattenMessages());
            return new ScriptResult();
        }
    }

    private async Task<string> GetScriptAsync()
    {
        var queryFolder = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var path = Path.Combine(queryFolder, "Queries", SelectedFile);
        var script = await File.ReadAllTextAsync(path);
        if (TableType == SymfactConstants.TableTypes[0]) return script.Replace(SymfactConstants.TableSuffixPlaceHolder, "");
        return script.Replace(SymfactConstants.TableSuffixPlaceHolder, TableType);
    }

    private async Task ExecuteSequentialAsync()
    {
        var results = new List<ScriptResult>();
        for (var i = 0; i < SequentialCount; i++)
        {
            results.Add(await ExecuteScriptAsync(i + 1, SequentialCount));
        }

        PrintResults(results);
    }

    private async Task ExecuteParallelAsync()
    {
        var tasks = new List<Task>();
        for (var i = 0; i < ParallelCount; i++)
        {
            tasks.Add(ExecuteScriptAsync(i + 1, ParallelCount));
        }

        await Task.WhenAll(tasks);
        PrintResults(tasks.Select(t => ((Task<ScriptResult>)t).Result));
    }

    private void PrintResults(IEnumerable<ScriptResult> results)
    {
        var timings = results.Where(r => r.Succeeded).Select(r => r.Ms).ToList();
        if (!timings.Any()) return;
        var max = timings.Max();
        var min = timings.Min();
        var avg = timings.Average();

        WriteMessage($"Total: {timings.Count} Avg: {avg}ms Fastest: {min}ms Slowest: {max}ms");
        _logger.Information("Total: {Total} Avg: {Avg}ms Fastest: {Min}ms Slowest: {Max}ms",
            timings.Count, avg, min, max);
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
                QueryFiles.Add(Path.GetFileName(file));
            }
            _logger.Debug("{FileCount} files found in folder '{QueryFolder}'",
                files.Length, queryFolder);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to open file picker for XSD files");
        }
    }

    private void WriteMessage(string message)
    {
        Messages.Add(message);
    }
}