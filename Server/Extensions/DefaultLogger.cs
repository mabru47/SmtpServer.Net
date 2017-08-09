using System;
using System.Collections.Generic;
using System.Text;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail.Extensions
{
    class DefaultLogger : ISmtpServerLogger
    {
        public void LogError(string value, params object[] args)
        {
            Console.WriteLine("ERROR:   " + value, args);
        }

        public void LogWarning(string value, params object[] args)
        {
            Console.WriteLine("Warning: " + value, args);
        }

        public void LogInfo(string value, params object[] args)
        {
            Console.WriteLine("INFO:    " + value, args);
        }

        public void LogRawSocketIn(string value, params object[] args)
        {
        }

        public void LogRawSocketOut(string value, params object[] args)
        {
        }
    }
}
