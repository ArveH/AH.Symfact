using System;
using System.Text;

namespace AH.Symfact.UI.Extensions;

public static class ExceptionExtensions
{
    public static string FlattenMessages(this Exception ex)
    {
        var allMessages = new StringBuilder(ex.Message);
        var tmpEx = ex;


        while (tmpEx?.InnerException != null)
        {
            tmpEx = tmpEx.InnerException;
            allMessages.Append(" ");
            allMessages.Append(tmpEx.Message);
            tmpEx = tmpEx.InnerException;
        }

        return allMessages.ToString();

    }
}