using System;
using System.Text.RegularExpressions;

namespace Tireless.Net.Mail.Commands
{
    class RcptCommand : CommandBase
    {
        public const String Command = "RCPT";

        public String Recipient
        {
            get; set;
        }

        public RcptCommand(String payload) :
            base(payload)
        {

        }

        public override string ParseParameter()
        {
            // To
            if (String.IsNullOrEmpty(this.Payload) == false)
            {
                var regexFrom = new Regex("TO:<(.+)>", RegexOptions.IgnoreCase);
                var matchesFrom = regexFrom.Matches(this.Payload);
                if (matchesFrom.Count == 1 && matchesFrom[0].Success == true)
                {
                    //TODO: Check if mail is valid
                    this.Recipient = matchesFrom[0].Groups[1].Value;
                }
            }
            if (this.Recipient == null)
                return "500 Command not recognized: TO:<>. Syntax error.";

            return null;
        }
    }
}
