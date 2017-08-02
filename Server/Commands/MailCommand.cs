using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tireless.Net.Mail.Commands
{
    class MailCommand : CommandBase
    {
        public static string Command = "MAIL";

        public String MailFrom
        {
            get; set;
        }

        public Int32? MailSize
        {
            get;
            set;
        }

        public Boolean Mail8BitMimeBody
        {
            get;
            set;
        }

        public MailCommand(String payload) :
            base(payload)
        {

        }

        public override String ParseParameter()
        {

            String paramString = null;
            //From
            if (String.IsNullOrEmpty(this.Payload) == false)
            {
                var regexFrom = new Regex("^FROM:<(.+)>", RegexOptions.IgnoreCase);
                var matchesFrom = regexFrom.Matches(this.Payload);
                if (matchesFrom.Count == 1 && matchesFrom[0].Success == true)
                {
                    this.MailFrom = matchesFrom[0].Groups[1].Value;
                    paramString = this.Payload.Substring(matchesFrom[0].Groups[0].Length);
                }
            }
            if (this.MailFrom == null)
                return "500 Command not recognized: FROM:<>. Syntax error.";

            //----------------------------------------------------------------------------------//

            var param = new Dictionary<String, String>();
            var regex = new Regex("^( (\\S+)=(\\S+))+$", RegexOptions.IgnoreCase);
            var matches = regex.Matches(paramString);
            if (matches.Count > 0)
            {
                for (int i = 0; i < matches[0].Groups[2].Captures.Count; i++)
                {
                    String key = matches[0].Groups[2].Captures[i].Value;
                    String value = matches[0].Groups[3].Captures[i].Value;

                    if (param.ContainsKey(key))
                        param[key.ToLowerInvariant()] = value;
                    else
                        param.Add(key.ToLowerInvariant(), value);
                }
            }

            //Size
            if (param.ContainsKey("size"))
            {
                if (false || Int64.TryParse(param["size"], out long size) == false)
                    return "500 Command not recognized: SIZE=. Syntax error.";

                if (size > 50 * 1024 * 1024 || size > Int32.MaxValue)
                    return "552 message size limit exceeded";

                this.MailSize = (Int32)size;
            }

            //BODY=8BITMIME
            if (param.ContainsKey("body"))
            {
                if (param["body"].ToLowerInvariant() == "8bitmime")
                    this.Mail8BitMimeBody = true;
            }
            return null;
        }
    }
}
