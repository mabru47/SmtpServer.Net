using Tireless.Net.Mail.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail.States
{
    class DataCompleteState : StateBase
    {
        private String host;
        private String mailFrom;
        private List<String> mailTo;
        private Guid mailGuid;
        private Stream emailStream;

        private IMailProcessor MailProcessorPlugin
        {
            get
            {
                return base.Server.GetPlugin<IMailProcessor>();
            }
        }

        public DataCompleteState(String host, String mailFrom, List<String> mailTo, Guid mailGuid, Stream emailStream, Connection connection) : base(connection)
        {
            this.host = host;
            this.mailFrom = mailFrom;
            this.mailTo = mailTo;
            this.mailGuid = mailGuid;
            this.emailStream = emailStream;
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            //Upload to xyz
            using (this.emailStream)
            {
                if (this.MailProcessorPlugin == null)
                    return new CloseState("550 Requested action not taken: mailbox plugin unavailable.", base.Connection);

                var processMailResult = await this.MailProcessorPlugin.ProcessAsync(this.mailFrom, this.mailTo.ToArray(), this.emailStream);
                if (processMailResult != null && processMailResult.IsError == true)
                    return new CloseState(processMailResult.ErrorText, base.Connection);

                await base.Connection.WriteLineAsync("250 OK: queued as " + this.mailGuid);

                while (true)
                {
                    var cmd = await base.GetNextCommand();

                    if (cmd is QuitCommand)
                        return new CloseState(base.Connection);
                    if (cmd is RstCommand)
                        return new WaitState(this.host, base.Connection);
                    if (cmd != null)
                        await base.Connection.WriteLineAsync("503 Need Quit or RST");
                }
            }
        }
    }
}
