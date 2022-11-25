using AH.Symfact.UI.Database;
using AH.Symfact.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

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
}