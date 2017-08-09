using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Tireless.Net.Mail.Models;
using Tireless.Net.Mail.Plugins;
using Tireless.Net.Mail.Extensions;
using System.IO;

namespace Tireless.Net.Mail
{
    public class Server
    {
        public ServerSettings Settings
        {
            get;
            private set;
        }

        internal X509Certificate2 X509Certificate
        {
            get;
            set;
        }

        private List<Connection> connections;

        private Dictionary<Type, Object> plugins;

        internal ISmtpServerLogger Logger
        {
            get;
            private set;
        }


        public Server()
            : this(new ServerSettings())
        {
        }

        public Server(ServerSettings settings)
        {
            this.Settings = settings;
            this.connections = new List<Connection>();
            this.plugins = new Dictionary<Type, Object>();
            this.Logger = new DefaultLogger();
        }

        private void Inizialize()
        {
            if (this.Settings.CertificatePath != null)
            {
                var cert = new X509Certificate2(this.Settings.CertificatePath);
                if (cert.NotAfter < DateTime.UtcNow || cert.NotBefore > DateTime.UtcNow)
                    this.Logger.LogWarning("The given certificate ist not valid!");
                else
                    this.Logger.LogInfo("Certificate with serial number " + cert.SerialNumber + " loaded");

                this.X509Certificate = cert;
            }
            else
            {
                this.Logger.LogWarning("No certificate specified.");
            }

            this.Logger.LogInfo("Temporary mail store: " + this.Settings.TempPath);
            if (Directory.Exists(this.Settings.TempPath) == false)
            {
                this.Logger.LogInfo("Creating directory \"" + this.Settings.TempPath + "\" for temporary mail store");
                Directory.CreateDirectory(this.Settings.TempPath);
            }
        }


        public void Run()
        {
            this.RunAsnyc().Wait();
        }

        public async Task RunAsnyc()
        {
            this.Inizialize();

            List<Task> t = new List<Task>();
            if (this.X509Certificate != null && this.Settings.SecurePort != null)
                t.Add(this.ListenAsync(this.Settings.SecurePort.Value, true));
            t.Add(this.ListenAsync(this.Settings.Port, false));

            await Task.WhenAll(t.ToArray());
        }

        private async Task ListenAsync(Int32 port, Boolean secure)
        {
            var tcpSocket = new TcpListener(this.Settings.Endpoint, port);

            tcpSocket.Start();
            TcpClient client = null;

            this.Logger.LogInfo("Server start listening on " + this.Settings.Endpoint + ":" + port);
            while ((client = await tcpSocket.AcceptTcpClientAsync()) != null)
            {
                this.Logger.LogInfo("Client endpoint: " + client.Client.RemoteEndPoint.ToString());

                var c = new Connection(this, client);
                var task = c.HandleClientAsnyc(secure);
                this.connections.Add(c);
            }

            this.Logger.LogInfo("Server stopped listening on " + this.Settings.Endpoint + ":" + port);
        }


        public async Task AddPluginAsync(IBlacklist blacklistPlugin)
        {
            this.Logger.LogInfo("Initializes blacklist plugin \"" + blacklistPlugin.GetType().FullName + "\".");
            if (blacklistPlugin is IInitializable initializable)
                await initializable.InitializeAsync();

            BlacklistCollection collection;
            if (this.plugins.ContainsKey(typeof(IBlacklist)))
                collection = (BlacklistCollection)this.plugins[typeof(IBlacklist)];
            else
                this.plugins.Add(typeof(IBlacklist), collection = new BlacklistCollection());

            collection.Add(blacklistPlugin);

            this.Logger.LogInfo("Plugin \"" + blacklistPlugin.GetType().FullName + "\" ready to use.");
        }

        public async Task AddPluginAsync(ICheckMailParams checkMailParamsPlugin)
        {
            this.Logger.LogInfo("Initializes CheckMailParamsPlugin \"" + checkMailParamsPlugin.GetType().FullName + "\".");
            if (checkMailParamsPlugin is IInitializable initializable)
                await initializable.InitializeAsync();

            CheckMailParamsCollection collection;
            if (this.plugins.ContainsKey(typeof(ICheckMailParams)))
                collection = (CheckMailParamsCollection)this.plugins[typeof(ICheckMailParams)];
            else
                this.plugins.Add(typeof(ICheckMailParams), collection = new CheckMailParamsCollection());

            collection.Add(checkMailParamsPlugin);

            this.Logger.LogInfo("Plugin \"" + checkMailParamsPlugin.GetType().FullName + "\" ready to use.");
        }

        public async Task AddPluginAsync(IMailProcessor mailProcessorPlugin)
        {
            this.Logger.LogInfo("Initializes MailProcessorPlugin \"" + mailProcessorPlugin.GetType().FullName + "\".");
            if (mailProcessorPlugin is IInitializable initializable)
                await initializable.InitializeAsync();

            MailProcessorCollection collection;
            if (this.plugins.ContainsKey(typeof(IMailProcessor)))
                collection = (MailProcessorCollection)this.plugins[typeof(IMailProcessor)];
            else
                this.plugins.Add(typeof(IMailProcessor), collection = new MailProcessorCollection());

            collection.Add(mailProcessorPlugin);

            this.Logger.LogInfo("Plugin \"" + mailProcessorPlugin.GetType().FullName + "\" ready to use.");
        }

        internal T GetPlugin<T>()
        {
            if (this.plugins.ContainsKey(typeof(T)))
                return (T)this.plugins[typeof(T)];
            return default(T);
        }


        public void AddLogger(ISmtpServerLogger logger)
        {
            this.Logger = logger ?? throw new ArgumentNullException();
        }
    }
}
