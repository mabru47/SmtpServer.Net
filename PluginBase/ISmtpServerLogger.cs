using System;
using System.Collections.Generic;
using System.Text;

namespace Tireless.Net.Mail.Plugins
{
    public interface ISmtpServerLogger
    {
        void LogInfo(String value, params object[] args);

        void LogWarning(String value, params object[] args);

        void LogError(String value, params object[] args);

        void LogRawSocketOut(String value, params object[] args);

        void LogRawSocketIn(String value, params object[] args);
    }
}
