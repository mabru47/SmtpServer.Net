using Tireless.Net.Mail.Commands;
using System;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail.States
{
    class MailFromState : StateBase
    {
        private String host;
        private String mailFrom;
        private Int32? mailSize;

        private IBlacklist BlacklistPlugin
        {
            get
            {
                return base.Server.GetPlugin<IBlacklist>();
            }
        }

        private ICheckMailParams CheckMailParamsPlugin
        {
            get
            {
                return base.Server.GetPlugin<ICheckMailParams>();
            }
        }

        public MailFromState(String host, MailCommand cmd, Connection connection) : base(connection)
        {
            this.host = host;

            this.mailFrom = cmd.MailFrom;
            this.mailSize = cmd.MailSize;

            this.NexCommands.Add(typeof(RcptCommand));
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            var blacklistResult = this.BlacklistPlugin != null ? await this.BlacklistPlugin.IsBlockedAsync(base.Connection.RemoteEndPoint.Address) : null;
            if (blacklistResult != null && blacklistResult.IsBlocked == true)
                return new CloseState(blacklistResult.ErrorText, base.Connection);

            var checkMailParamResult = this.CheckMailParamsPlugin != null ? await this.CheckMailParamsPlugin.CheckFromAsync(this.mailFrom) : null;
            if (checkMailParamResult != null && checkMailParamResult.IsError == true)
                return new WaitState(this.host, checkMailParamResult.ErrorText, base.Connection);

            //----------------------------------------------------------------------------------//

            await base.Connection.WriteLineAsync("250 ok");

            while (true)
            {
                var cmd = await base.GetNextCommand();

                if (cmd is RcptCommand)
                    return new MailRecipientsState(this.host, this.mailFrom, this.mailSize, (RcptCommand)cmd, base.Connection);
                if (cmd is QuitCommand)
                    return new CloseState(base.Connection);
                if (cmd is RstCommand)
                    return new WaitState(this.host, base.Connection);
                if (cmd != null)
                    await base.Connection.WriteLineAsync("503 Need RCPT TO: first");
            }
        }
    }
}
