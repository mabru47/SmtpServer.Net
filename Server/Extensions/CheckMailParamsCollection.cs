using System.Collections.Generic;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail.Extensions
{
    class CheckMailParamsCollection : ICheckMailParams
    {
        private List<ICheckMailParams> allPlugins;

        public CheckMailParamsCollection()
        {
            this.allPlugins = new List<ICheckMailParams>();
        }

        public void Add(ICheckMailParams plugin)
        {
            this.allPlugins.Add(plugin);
        }

        public async Task<CheckMailParamsResult> CheckFromAsync(string from)
        {
            foreach (var item in this.allPlugins)
            {
                var r = await item.CheckFromAsync(from);
                if (r.IsError)
                    return r;
            }
            return null;
        }

        public async Task<CheckMailParamsResult> CheckRcptAsync(string from, string rcpt)
        {
            foreach (var item in this.allPlugins)
            {
                var r = await item.CheckRcptAsync(from, rcpt);
                if (r.IsError)
                    return r;
            }
            return null;
        }
    }
}