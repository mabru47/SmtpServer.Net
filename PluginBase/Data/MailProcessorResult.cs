using System;

namespace Tireless.Net.Mail.Plugins
{
    public class MailProcessorResult
    {
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
        
        public MailProcessorResult()
        {
        }

        public MailProcessorResult(String error)
        {
            this.ErrorText = error;
        }
    }
}
