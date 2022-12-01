using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace AH.Symfact.Cmd;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("\n\nUsage: AH.Symfact.Cmd.exe <runCount> <script> <connection string>\n");
            return;
        }

        var results = new ConcurrentBag<ScriptResult>();
        var threadIds = new ConcurrentBag<int>();
        var runCount = Convert.ToInt32(args[0]);
        var path = args[1];
        var connectionString = args[2];
        var toInclusive = runCount + 1;
        Console.WriteLine($"Executing Script '{path}'...");
        Parallel.For(1, toInclusive, (i) =>
        {
            results.Add(ExecuteScript(i, runCount, path, connectionString));
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
        });
        foreach (var res in results.OrderBy(r => r.Ms))
        {
            Console.WriteLine($"ThreadId {res.ThreadId,3}: {res.Ms,6}");
        }
        PrintResult(results, threadIds);
    }

    private static ScriptResult ExecuteScript(int index, int total, string path, string connectionString)
    {
        try
        {
            var script = File.ReadAllText(path).Replace("<<<TABLESUFFIX>>>", "ExtractedColumns");
            var result = ExecuteQuery(script, connectionString);
            result.ThreadId = Thread.CurrentThread.ManagedThreadId;
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Execution failed! ({1} of {2}). " + ex.Message,
                index, total);
            return new ScriptResult();
        }
    }

    private static ScriptResult ExecuteQuery(string script, string connectionString)
    {
        var sw = new Stopwatch();
        sw.Start();
        using var dbConn = new SqlConnection(connectionString);
        dbConn.Open();
        using var cmd = dbConn.CreateCommand();
        cmd.CommandText = "SELECT \r\n" +
                          "    c.DocName, \r\n" +
                          "    op.Initials\r\n" +
                          "FROM ContractNoSchema c\r\n" +
                          "JOIN OrganisationalPersonNoSchema op \r\n" +
                          "    ON op.Cn = c.ContractOwnerCN\r\n" +
                          "WHERE c.Status != 'deleted'\r\n" +
                          "  AND c.ContractType = 'Insurance'";
        using var reader = cmd.ExecuteReader();
        //while (reader.Read())
        //{
        //    var docName = reader.GetString(0);
        //    var xml = reader.GetString(1);
        //}
        reader.Close();
        dbConn.Close();
        sw.Stop();
        return new ScriptResult(sw.ElapsedMilliseconds);
    }


    private static void PrintResult(IEnumerable<ScriptResult> results, IEnumerable<int>? threadIds=null)
    {
        var timings = results.Where(r => r.Succeeded).Select(r => r.Ms).ToList();
        if (!timings.Any()) return;
        var max = timings.Max();
        var min = timings.Min();
        var avg = timings.Average();

        Console.WriteLine($"Total: {timings.Count} Avg: {avg}ms Fastest: {min}ms Slowest: {max}ms");

        if (threadIds != null)
        {
            Console.WriteLine($"{threadIds.Distinct().Count()} distinct threads");
        }
    }
}