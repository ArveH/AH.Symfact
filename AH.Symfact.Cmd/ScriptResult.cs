namespace AH.Symfact.Cmd;

public class ScriptResult
{
    public ScriptResult(long ms = 0)
    {
        Ms = ms;
        Succeeded = true;
    }

    public ScriptResult()
    {
    }

    public long Ms { get; set; }
    public int ThreadId { get; set; }
    public bool Succeeded { get; set; }
}