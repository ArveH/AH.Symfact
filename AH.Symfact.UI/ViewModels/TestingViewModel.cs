using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using AH.Symfact.UI.Models;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Windows.UI.Core;

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
    }

    public CoreDispatcher? Dispatcher { get; set; }
    public DispatcherQueue? DispatcherQueue { get; set; }

    [ObservableProperty]
    private string _selectedFile = string.Empty;
    [ObservableProperty]
    private string _tableType = SymfactConstants.TableTypes[2];
    [ObservableProperty]
    private string _logMessage = string.Empty;
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

    private ScriptResult ExecuteScript(int index, int threadId, int total)
    {
        try
        {
            var script = GetScript();
            var sw = new Stopwatch();
            _logger.Debug("Executing Script '{FileName}' ({Index} of {Total}) ...",
                SelectedFile, index, total);
            sw.Start();
            var rows = _dbCommands.ExecuteQuery(script);
            sw.Stop();
            _logger.Information("Script '{FileName}' returned {Rows} rows and executed in {Ms}ms ({Index} of {Total}, ThreadId: {ThreadId}) ",
                SelectedFile, rows, sw.ElapsedMilliseconds, index, total, threadId);
            WriteMessage($"(Script '{SelectedFile}' returned {rows} rows and executed in {sw.ElapsedMilliseconds}ms ({index} of {total}, ThreadId: {threadId}) ");
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

    private string GetScript()
    {
        var queryFolder = WeakReferenceMessenger.Default.Send<DataFolderChangedMessage>();
        var path = Path.Combine(queryFolder, "Queries", SelectedFile);
        var script = File.ReadAllText(path);
        if (TableType == SymfactConstants.TableTypes[0]) return script.Replace(SymfactConstants.TableSuffixPlaceHolder, "");
        return script.Replace(SymfactConstants.TableSuffixPlaceHolder, TableType);
    }

    private async Task ExecuteSequentialAsync()
    {
        var results = new List<ScriptResult>();
        for (var i = 0; i < SequentialCount; i++)
        {
            await Task.Run(() =>
            {
                results.Add(ExecuteScript(i + 1, Thread.CurrentThread.ManagedThreadId, SequentialCount));
            });
        }

        _logger.Information("Sequential: {LogMessage}", LogMessage);
        PrintResult(results);
    }

    private async Task ExecuteParallelAsync()
    {
        var results = new ConcurrentBag<ScriptResult>();
        var threadIds = new ConcurrentBag<int>();
        var cnt = ParallelCount + 1;
        await Task.Run(() =>
        {
            Parallel.For(1, cnt, (i) =>
            {
                results.Add(ExecuteScript(i, Thread.CurrentThread.ManagedThreadId, ParallelCount));
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            });
        });

        _logger.Information("Parallel: {LogMessage}", LogMessage);
        PrintResult(results, threadIds);
    }

    private void PrintResult(IEnumerable<ScriptResult> results, IEnumerable<int>? threadIds=null)
    {
        var timings = results.Where(r => r.Succeeded).Select(r => r.Ms).ToList();
        if (!timings.Any()) return;
        var max = timings.Max();
        var min = timings.Min();
        var avg = timings.Average();

        WriteMessage($"Total: {timings.Count} Avg: {avg}ms Fastest: {min}ms Slowest: {max}ms");
        _logger.Information("Total: {Total} Avg: {Avg}ms Fastest: {Min}ms Slowest: {Max}ms",
            timings.Count, avg, min, max);

        if (threadIds != null)
        {
            _logger.Information("{DistinctThreadIds} distinct threads",
                threadIds.Distinct().Count());
        }
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
        DispatcherQueue?.TryEnqueue(() =>
        {
            Messages.Add(message);
        });
    }
}