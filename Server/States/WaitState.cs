using Tireless.Net.Mail.Commands;
using System;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.States
{
    class WaitState : StateBase
    {
        private String host;
        private Boolean isExtendedSMTP;
        private Boolean fromRst;
        private String errorText;

        private WaitState(Connection connection) : base(connection)
        {
            this.NexCommands.Add(typeof(QuitCommand));
            this.NexCommands.Add(typeof(MailCommand));
            this.NexCommands.Add(typeof(StartTlsCommand));
            this.NexCommands.Add(typeof(HeloCommand));
        }

        public WaitState(HeloCommand cmd, Connection connection) : this(connection)
        {
            this.host = cmd.Host;
            this.isExtendedSMTP = false;
        }

        public WaitState(EhloCommand cmd, Connection connection) : this(connection)
        {
            this.host = cmd.Host;
            this.isExtendedSMTP = true;
        }

        public WaitState(String host, Connection connection) : this(connection)
        {
            this.host = host;
            this.fromRst = true;
        }

        public WaitState(String host, String errorText, Connection connection) : this(connection)
        {
            this.host = host;
            this.errorText = errorText;
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            if (errorText != null)
            {
                await base.Connection.WriteLineAsync(errorText);
            }
            else if (fromRst)
            {
                await base.Connection.WriteLineAsync("250 OK");
            }
            else
            {
                if (isExtendedSMTP)
                {
                    await base.Connection.WriteLineAsync("250-mx.m47.einfachkrank.de Hello [" + this.Connection.RemoteEndPoint + "]");
                    await base.Connection.WriteLineAsync("250-SIZE " + (50 * 1024 * 1024));
                    await base.Connection.WriteLineAsync("250-8BITMIME");
                    await base.Connection.WriteLineAsync("250-PIPELINING");
                    if (this.Server.X509Certificate != null)
                        await base.Connection.WriteLineAsync("250-STARTTLS");
                    await base.Connection.WriteLineAsync("250 OK");
                }
                else
                    await base.Connection.WriteLineAsync("250 mx.m47.einfachkrank.de Hello [" + this.Connection.RemoteEndPoint + "]");
            }

            while (true)
            {
                var cmd = await base.GetNextCommand();

                if (cmd is MailCommand)
                    return new MailFromState(this.host, (MailCommand)cmd, this.Connection);
                if (cmd is QuitCommand)
                    return new CloseState(base.Connection);
                if (cmd is StartTlsCommand)
                    return new StartTlsState((StartTlsCommand)cmd, base.Connection);
                if (cmd is HeloCommand)
                    return new WaitState((HeloCommand)cmd, base.Connection);
                if (cmd is RstCommand)
                    await base.Connection.WriteLineAsync("250 OK");
                else if (cmd != null)
                    await base.Connection.WriteLineAsync("503 Need Mail From: first");
            }
        }
    }
}
