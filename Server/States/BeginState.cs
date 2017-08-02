using Tireless.Net.Mail.Commands;
using System;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.States
{
    class BeginState : StateBase
    {
        private Boolean fromSslUpgrade;

        public BeginState(Connection connection) : base(connection)
        {
            this.NexCommands.Add(typeof(HeloCommand));
            this.NexCommands.Add(typeof(EhloCommand));
            this.NexCommands.Add(typeof(QuitCommand));
        }
        public BeginState(Boolean fromSslUpgrade, Connection connection) : this(connection)
        {
            this.fromSslUpgrade = fromSslUpgrade;
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            if (!fromSslUpgrade)
                await base.Connection.WriteLineAsync("220 Mail47 SMTP Service. Ready for action at " + DateTime.UtcNow.ToString("r"));
            //else
            //    await base.Connection.WriteLineAsync("250 OK");

            while (true)
            {
                var cmd = await base.GetNextCommand();

                if (cmd is HeloCommand)
                    return new WaitState((HeloCommand)cmd, base.Connection);
                if (cmd is EhloCommand)
                    return new WaitState((EhloCommand)cmd, base.Connection);
                if (cmd is QuitCommand)
                    return new CloseState(base.Connection);
                if (cmd is RstCommand)
                    await base.Connection.WriteLineAsync("250 OK");
                else if (cmd != null)
                    await base.Connection.WriteLineAsync("503 Be polite and say hello first");
            }
        }
    }
}
