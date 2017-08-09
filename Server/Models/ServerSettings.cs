using System;
using System.IO;
using System.Net;

namespace Tireless.Net.Mail.Models
{
    public class ServerSettings
    {
        public Int32 Port
        {
            get;
            set;
        }

        public Int32? SecurePort
        {
            get;
            set;
        }

        public IPAddress Endpoint
        {
            get;
            set;
        }

        public String CertificatePath
        {
            get;
            set;
        }

        public String ServiceName
        {
            get;
            set;
        }

        public String ServiceDomain
        {
            get;
            set;
        }

        public String InstanceID
        {
            get;
            set;
        }

        public String TempPath
        {
            get;
            set;
        }

        public ServerSettings()
        {
            this.Endpoint = IPAddress.Parse("127.0.0.1");
            this.Port = 25;
            this.ServiceDomain = "mx.example.org";
            this.ServiceName = "ExampleService";
            this.InstanceID = Guid.NewGuid().ToString();
            this.TempPath = Path.GetTempPath() + "mails" + Path.DirectorySeparatorChar;
        }
    }
}
