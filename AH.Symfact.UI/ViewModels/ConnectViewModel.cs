using AH.Symfact.UI.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Serilog;
using System;
using System.Threading.Tasks;
using AH.Symfact.UI.SqlServer;
using MongoDB.Driver;

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
        _logger = logger.ForContext<ConnectViewModel>();
        _sqlConnectionString = _dbConnFactory.SqlConnectionString.ConnectionString ?? "";
    }

    [ObservableProperty]
    private string _sqlConnectionString;

    partial void OnSqlConnectionStringChanging(string value)
    {
        _dbConnFactory.SqlConnectionString.ConnectionString = value;
    }

    [ObservableProperty]
    private string _connectionStatus = "Ready...";

    [RelayCommand]
    public async Task ConnectSqlAsync()
    {
        var csBuilder = GetConnectionStringBuilder(SqlConnectionString);
        if (csBuilder == null)
        {
            _logger.Error("Invalid ConnectionString '{ConnectionString}'",
                SqlConnectionString);
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
                        SqlConnectionString);
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

    [RelayCommand]
    public async Task ConnectMongoAsync()
    {
        var dbClient = new MongoClient();
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