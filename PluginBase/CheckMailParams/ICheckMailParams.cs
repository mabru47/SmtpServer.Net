using System;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.Plugins
{
    public interface ICheckMailParams
    {
        Task<CheckMailParamsResult> CheckFromAsync(String from);

        Task<CheckMailParamsResult> CheckRcptAsync(String from, String rcpt);
    }
}
