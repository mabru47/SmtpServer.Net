using System;

namespace Tireless.Net.Mail.Commands
{
    class RstCommand : CommandBase
    {
        public const String Command = "RST";
        public RstCommand(String payload) :
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
