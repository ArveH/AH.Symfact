﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AH.Symfact.UI.Models;

namespace AH.Symfact.UI.Database;

public interface IDbCommands
{
    Task<List<string>> GetAllTablesAsync();
    Task DeleteTablesAsync(IEnumerable<string> tableNames);
    Task ExecuteScriptAsync(string script);
    Task<int> InsertRowsAsync(string tableName, IEnumerable<TableRow>? input);
}