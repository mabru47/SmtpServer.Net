using System.Collections.Generic;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;
using System.IO;
using System;

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

        public async Task<MailProcessorResult> ProcessAsync(string from, string[] rcpt, Guid mailGuid, Stream data)
        {
            foreach (var item in this.allPlugins)
            {
                var r = await item.ProcessAsync(from, rcpt, mailGuid, data);
                if (r.IsError)
                    return r;
            }
            return null;
        }
    }
}