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
    private readonly IDbConnFactory _dbConnFactory;
    private readonly ILogger _logger;

    public ConnectViewModel(
        IDbConnFactory dbConnFactory,
        ILogger logger)
    {
        _dbConnFactory = dbConnFactory;
        _logger = logger.ForContext<MenuControl>();
        _connectionString = _dbConnFactory.DbConnectionString.ConnectionString ?? "";
        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
    }

    [ObservableProperty]
    private string _connectionString;

    partial void OnConnectionStringChanging(string value)
    {
        _dbConnFactory.DbConnectionString.ConnectionString = value;
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
            await using var dbConn = _dbConnFactory.CreateConnection();

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