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
            Console.WriteLine("Usage: AH.Symfact.Cmd.exe <runCount> <script> <connection string>");
        }

        var results = new ConcurrentBag<ScriptResult>();
        var threadIds = new ConcurrentBag<int>();
        var runCount = Convert.ToInt32(args[0]);
        var path = args[1];
        var connectionString = args[2];
        var toInclusive = runCount + 1;
        Parallel.For(1, toInclusive, (i) =>
        {
            results.Add(ExecuteScript(i, Thread.CurrentThread.ManagedThreadId, runCount, path, connectionString));
            threadIds.Add(Thread.CurrentThread.ManagedThreadId);
        });
    }

    private static ScriptResult ExecuteScript(int index, int threadId, int total, string path, string connectionString)
    {
        try
        {
            var script = File.ReadAllText(path);
            var fileName = Path.GetFileName(path);
            Console.WriteLine($"Executing Script '{fileName}' ({index} of {total}) ...");
            var sw = new Stopwatch();
            sw.Start();
            var rows = ExecuteQuery(script, connectionString);
            sw.Stop();
            Console.WriteLine("Script '{0}' returned {1} rows and executed in {2}ms ({3} of {4}, ThreadId: {5}) ",
                fileName, rows, sw.ElapsedMilliseconds, index, total, threadId);
            return new ScriptResult(sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Executing script '{0}' ({1} of {2}) failed. " + ex.Message,
                Path.GetFileName(path), index, total);
            return new ScriptResult();
        }
    }

    private static int ExecuteQuery(string script, string connectionString)
    {
        using var dbConn = new SqlConnection(connectionString);
        dbConn.Open();
        var server = new Server(new ServerConnection(dbConn));
        var result = server.ConnectionContext.ExecuteWithResults(script);
        return result.Tables[0].Rows.Count;
    }
}