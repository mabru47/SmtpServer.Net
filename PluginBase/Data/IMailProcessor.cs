using System;
using System.IO;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.Plugins
{
    public interface IMailProcessor
    {
        Task<MailProcessorResult> ProcessAsync(String from, String[] rcpt, Stream data);
    }
}
