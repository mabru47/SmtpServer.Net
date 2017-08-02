using System;
using System.Net;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.Plugins
{
    public interface IBlacklist
    {
        Task<CheckBlacklistResult> IsBlockedAsync(IPAddress client);

        void AddBlockingRule(IPAddress client, String rule);
    }
}
