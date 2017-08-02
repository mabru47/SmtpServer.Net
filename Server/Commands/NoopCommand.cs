using System;

namespace Tireless.Net.Mail.Commands
{
    class NoopCommand : CommandBase
    {
        public const String Command = "NOOP";
        public NoopCommand(String payload) :
            base(payload)
        {

        }

        public override string ParseParameter()
        {
            if (String.IsNullOrEmpty(base.Payload) == false)
                return "501 Syntax error, no parameters allowed.";
            return null;
        }
    }
}
