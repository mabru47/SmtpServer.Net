using System;

namespace Tireless.Net.Mail.Plugins
{
    public class CheckBlacklistResult
    {
        public Boolean IsBlocked
        {
            get
            {
                return !String.IsNullOrEmpty(this.ErrorText);
            }
        }

        public String ErrorText { get; set; }

        public CheckBlacklistResult(String errorText)
        {
            this.ErrorText = errorText;
        }

        public CheckBlacklistResult()
        {
        }
    }
}
