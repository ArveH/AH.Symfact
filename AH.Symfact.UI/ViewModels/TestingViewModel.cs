using AH.Symfact.UI.Database;
using AH.Symfact.UI.Services;
using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.IO;

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
    }

    [ObservableProperty]
    private string _selectedFile = string.Empty;
    [ObservableProperty]
    private ObservableCollection<string> _queryFiles = new();

    public void QueryFileChanged(string? queryFile)
    {
        if (!string.IsNullOrWhiteSpace(queryFile))
        {
            SelectedFile = queryFile;
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