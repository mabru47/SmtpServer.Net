using System;

namespace Tireless.Net.Mail.Commands
{
    class EhloCommand : CommandBase
    {
        public const String Command = "EHLO";
        public String Host
        {
            get;
            private set;
        }

        public EhloCommand(String payload) :
            base(payload)
        {

        }

        public override string ParseParameter()
        {
            if (String.IsNullOrEmpty(this.Payload) || (Uri.CheckHostName(this.Payload) & ~UriHostNameType.Basic) == 0)
                return "500 Command not recognized: host. Syntax error.";
            this.Host = this.Payload;
            return null;
        }
    }
}
