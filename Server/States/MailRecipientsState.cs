using Tireless.Net.Mail.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail.States
{
    class MailRecipientsState : StateBase
    {
        private String host;
        private String mailFrom;
        private Int32? mailSize;
        private List<String> mailRecipients;
        private RcptCommand initCommand;

        private ICheckMailParams CheckMailParamsPlugin
        {
            get
            {
                return base.Server.GetPlugin<ICheckMailParams>();
            }
        }

        public MailRecipientsState(String host, String mailFrom, Int32? mailSize, RcptCommand cmd, Connection connection) : base(connection)
        {
            this.host = host;
            this.mailFrom = mailFrom;
            this.mailSize = mailSize;
            this.initCommand = cmd;
            this.mailRecipients = new List<String>(1);

            this.NexCommands.Add(typeof(RcptCommand));
            this.NexCommands.Add(typeof(DataCommand));
            this.NexCommands.Add(typeof(QuitCommand));
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            await this.HandleCommandAsync(this.initCommand);

            while (true)
            {
                CommandBase cmd = await base.GetNextCommand();
                if (cmd is RcptCommand)
                    await this.HandleCommandAsync((RcptCommand)cmd);
                else if (cmd is DataCommand)
                    return new ReceiveDataState(this.host, this.mailFrom, this.mailSize, this.mailRecipients, (DataCommand)cmd, base.Connection);
                else if (cmd is QuitCommand)
                    return new CloseState(base.Connection);
                else if (cmd is RstCommand)
                    return new WaitState(this.host, base.Connection);
                else if (cmd != null)
                    await base.Connection.WriteLineAsync("503 Need RCPT TO: or DATA");
            }
        }

        private async Task HandleCommandAsync(RcptCommand cmd)
        {
            var checkMailParamsResult = this.CheckMailParamsPlugin != null ? await this.CheckMailParamsPlugin.CheckRcptAsync(this.mailFrom, cmd.Recipient) : null;
            if (checkMailParamsResult != null && checkMailParamsResult.IsError == true)
            {
                await base.Connection.WriteLineAsync(checkMailParamsResult.ErrorText);
            }
            else
            {
                this.mailRecipients.Add(cmd.Recipient);
                await base.Connection.WriteLineAsync("250 OK");
            }
        }
    }
}
