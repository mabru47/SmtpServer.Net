using System;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.Commands
{
    abstract class CommandBase
    {
        public String Payload
        {
            get;
            protected set;
        }

        public CommandBase(String payload)
        {
            this.Payload = payload;
        }

        public virtual Task<String> ParseParameterAsync(Connection connection)
        {
            return Task.FromResult<String>(null);
        }

        public virtual String ParseParameter()
        {
            return null;
        }
    }
}
