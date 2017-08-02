using System;

namespace Tireless.Net.Mail.Commands
{
    class InvalidCommand : CommandBase
    {
        public String Line
        {
            get; private set;
        }

        public InvalidCommand(String payload) :
            base(payload)
        {

        }

        public override string ParseParameter()
        {
            return null;
        }
    }
}
