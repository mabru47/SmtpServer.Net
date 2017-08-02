using System.Collections.Generic;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;
using System.IO;

namespace Tireless.Net.Mail.Extensions
{
    class MailProcessorCollection : IMailProcessor
    {
        private List<IMailProcessor> allPlugins;

        public MailProcessorCollection()
        {
            this.allPlugins = new List<IMailProcessor>();
        }

        public void Add(IMailProcessor plugin)
        {
            this.allPlugins.Add(plugin);
        }

        public async Task<MailProcessorResult> ProcessAsync(string from, string[] rcpt, Stream data)
        {
            foreach (var item in this.allPlugins)
            {
                var r = await item.ProcessAsync(from, rcpt, data);
                if (r.IsError)
                    return r;
            }
            return null;
        }
    }
}