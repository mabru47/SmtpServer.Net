using Tireless.Net.Mail.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.States
{
    abstract class StateBase
    {
        protected Connection Connection
        {
            get;
            private set;
        }
        protected List<Type> NexCommands
        {
            get;
            private set;
        }
        protected SmtpServer Server
        {
            get
            {
                return this.Connection.Server;
            }
        }

        public StateBase(Connection connection)
        {
            this.Connection = connection;
            this.NexCommands = new List<Type>
            {
                typeof(RstCommand),
                typeof(QuitCommand),
                typeof(NoopCommand)
            };
        }

        public abstract Task<StateBase> HandleStateAsync();

        protected async Task<CommandBase> GetNextCommand()
        {
            var line = await this.Connection.ReadLineAsync();
            var cmd = await this.GetCommandAsync(line, this.NexCommands);

            if (cmd is NoopCommand)
            {
                await this.Connection.WriteLineAsync("250 OK");
                return null;
            }

            return cmd;
        }

        protected async Task<CommandBase> GetCommandAsync(String line, List<Type> possibleCommands)
        {
            if (String.IsNullOrEmpty(line))
                return null;

            CommandBase cmd = null;
            String payload = null;
            if ((payload = this.GetPayloadByCommand(line, HeloCommand.Command)) != null)
                cmd = new HeloCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, EhloCommand.Command)) != null)
                cmd = new EhloCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, StartTlsCommand.Command)) != null)
                cmd = new StartTlsCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, MailCommand.Command)) != null)
                cmd = new MailCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, RcptCommand.Command)) != null)
                cmd = new RcptCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, DataCommand.Command)) != null)
                cmd = new DataCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, QuitCommand.Command)) != null)
                cmd = new QuitCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, RstCommand.Command)) != null)
                cmd = new RstCommand(payload);
            else if ((payload = this.GetPayloadByCommand(line, NoopCommand.Command)) != null)
                cmd = new NoopCommand(payload);

            if (cmd != null)
            {
                if (possibleCommands.Contains(cmd.GetType()))
                {
                    var error = cmd.ParseParameter();
                    if (error == null)
                        error = await cmd.ParseParameterAsync(this.Connection);

                    if (error != null)
                    {
                        await this.Connection.WriteLineAsync(error);
                        cmd = null;
                    }
                }
                else
                    cmd = new InvalidCommand(payload);
            }
            else
                await this.Connection.WriteLineAsync("500 Unrecognized command");

            return cmd;
        }

        private String GetPayloadByCommand(String line, String command)
        {
            if (line.Length >= command.Length)
            {
                if (line.Substring(0, command.Length).ToLowerInvariant() == command.ToLowerInvariant())
                {
                    if (line.Length > command.Length)
                    {
                        if (line[command.Length] == ' ')
                            return line.Substring(command.Length + 1);
                    }
                    else
                        return String.Empty;
                }
            }
            return null;
        }
    }
}
