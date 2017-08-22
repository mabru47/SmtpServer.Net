using System;

namespace Tireless.Net.Mail.Plugins
{
    public class MailProcessorResult
    {
        public static readonly MailProcessorResult Okay = new MailProcessorResult();

        public static MailProcessorResult FromError(int code, String description)
        {
            return new MailProcessorResult(code + " " + description);
        }

        public Boolean IsError
        {
            get
            {
                return !String.IsNullOrEmpty(this.ErrorText);
            }
        }

        public String ErrorText
        {
            get;
            set;
        }

        private MailProcessorResult()
        {
        }

        private MailProcessorResult(String error)
        {
            this.ErrorText = error;
        }
    }
}
