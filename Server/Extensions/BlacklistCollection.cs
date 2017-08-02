using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail.Extensions
{
    class BlacklistCollection : IBlacklist
    {
        private List<IBlacklist> allPlugins;

        public BlacklistCollection()
        {
            this.allPlugins = new List<IBlacklist>();
        }

        public void Add(IBlacklist plugin)
        {
            this.allPlugins.Add(plugin);
        }

        public void AddBlockingRule(IPAddress client, String rule)
        {
            foreach (var plugin in this.allPlugins)
                plugin.AddBlockingRule(client, rule);
        }

        public async Task<CheckBlacklistResult> IsBlockedAsync(IPAddress client)
        {
            CheckBlacklistResult result = null;
            foreach (var plugin in this.allPlugins)
                if ((result = await plugin.IsBlockedAsync(client)) != null)
                    return result;
            return null;
        }
    }
}