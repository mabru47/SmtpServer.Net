using System;

namespace Tireless.Net.Mail.Plugins
{
    public class CheckMailParamsResult
    {
        public Boolean IsError
        {
            get
            {
                return !String.IsNullOrEmpty(this.ErrorText);
            }
        }

        public String ErrorText { get; set; }

        public CheckMailParamsResult()
        {
        }

        public CheckMailParamsResult(String error) : this()
        {
            this.ErrorText = error;
        }

    }
}
