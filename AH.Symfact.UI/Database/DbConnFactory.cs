﻿using Serilog;

namespace AH.Symfact.UI.Database;

public class DbConnFactory : IDbConnFactory
{
    private readonly ILogger _logger;

    public DbConnFactory(
        SqlConnectionString sqlConnectionString,
        ILogger logger)
    {
        SqlConnectionString = sqlConnectionString;
        _logger = logger;
    }

    public SqlConnectionString SqlConnectionString { get; }

    public IDbConn CreateConnection()
    {
        return new DbConn(SqlConnectionString, _logger);
    }
}