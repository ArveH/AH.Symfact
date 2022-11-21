using AH.Symfact.UI.Controls;
using AH.Symfact.UI.Database;
using AH.Symfact.UI.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AH.Symfact.UI.ViewModels;

public partial class ConnectViewModel : ObservableObject
{
    private readonly DbConnectionString _dbConnectionString;
    private readonly ILogger _logger;

    public ConnectViewModel(
        DbConnectionString dbConnectionString,
        ILogger logger)
    {
        _dbConnectionString = dbConnectionString;
        _logger = logger.ForContext<MenuControl>();
        _connectionString = _dbConnectionString.ConnectionString ?? "";
        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
    }

    [ObservableProperty]
    private string _connectionString;

    partial void OnConnectionStringChanging(string value)
    {
        _dbConnectionString.ConnectionString = value;
    }

    [ObservableProperty]
    private string _connectionStatus = "Ready...";

    public ICommand ConnectCommand { get; }

    private async Task ConnectAsync()
    {
        var csBuilder = GetConnectionStringBuilder(ConnectionString);
        if (csBuilder == null)
        {
            _logger.Error("Invalid ConnectionString '{ConnectionString}'",
                ConnectionString);
            ConnectionStatus = "Invalid ConnectionString";
            return;
        }

        _logger.Information("Connecting...");
        ConnectionStatus = "Connecting...";
        try
        {
            await using IDbConn dbConn = new DbConn(_dbConnectionString, _logger);

            if (await dbConn.ConnectAsync())
            {
                var database = dbConn.DbName;
                if (string.IsNullOrWhiteSpace(database))
                {
                    _logger.Error("Can't get DbName from '{ConnectionString}'",
                        ConnectionString);
                    ConnectionStatus = "Can't get DbName";
                    return;
                }

                _logger.Information("Connection to '{Database}' worked", database);
                ConnectionStatus = $"Connection to '{database}' worked";
            }
            else
            {
                if (!csBuilder.TryGetValue("Database", out var dbName))
                {
                    dbName = "<null>";
                }
                _logger.Error("Failed to connect to database '{Database}'", dbName);
                ConnectionStatus = $"Failed to connect to database '{dbName}'";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Connection failed");
            ConnectionStatus = ex.FlattenMessages();
        }
    }

    private SqlConnectionStringBuilder? GetConnectionStringBuilder(string connectionString)
    {
        try
        {
            return new SqlConnectionStringBuilder(connectionString);
        }
        catch (Exception)
        {
            return null;
        }
    }
}