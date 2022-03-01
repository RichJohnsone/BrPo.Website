using System;
using System.Text;

namespace BrPo.Website.Services.Email;

public static class ExceptionExtensions
{
    public static string ToMessageString(this Exception e)
    {
        var sb = new StringBuilder();
        sb.AppendLine(e.Message);
        var innerMessages = string.Empty;
        GetInnerMessages(innerMessages, e);
        sb.AppendLine(innerMessages);

        static string GetInnerMessages(string messages, Exception ex)
        {
            if (ex.InnerException == null) return messages;
            messages += ex.InnerException.Message + "\r\n";
            return GetInnerMessages(messages, ex.InnerException);
        }

        return sb.ToString();
    }
}