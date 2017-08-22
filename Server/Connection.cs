using Tireless.Net.Mail.Exceptions;
using Tireless.Net.Mail.Helper;
using Tireless.Net.Mail.States;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tireless.Net.Mail.Plugins;

namespace Tireless.Net.Mail
{
    class Connection
    {
        private TcpClient client;
        private NetworkStream networkStream;
        private SslStream sslStream;
        private StreamLimiter streamLimiter;
        private StreamReader streamReader;

        private StateBase state;

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return (IPEndPoint)client.Client.RemoteEndPoint;
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                return (IPEndPoint)client.Client.LocalEndPoint;
            }
        }

        public Boolean IsSecureSocket
        {
            get;
            private set;
        }

        public SmtpServer Server
        {
            get;
            private set;
        }

        private ISmtpServerLogger Logger
        {
            get
            {
                return this.Server.Logger;
            }
        }

        //----------------------------------------------------------------------//

        public Connection(SmtpServer server, TcpClient client)
        {
            this.Server = server;
            this.client = client;
            this.IsSecureSocket = false;
        }

        public async Task HandleClientAsnyc(Boolean startSsl)
        {
            try
            {
                this.networkStream = this.client.GetStream();

                if (startSsl)
                    await this.UpgradeConnectionAsync();
                else
                {
                    this.streamLimiter = new StreamLimiter(networkStream, 150 * 1024 * 1024);
                    this.streamReader = new StreamReader(this.streamLimiter, Encoding.UTF8, false, 4069, true);
                }

                this.state = new BeginState(this);
                do
                {
                    try
                    {
                        this.state = await this.state.HandleStateAsync();
                    }
                    catch (TimeoutException)
                    {
                        this.state = new CloseState(this);
                    }
                    catch (SecurityException)
                    {
                        this.state = new CloseState(this);
                    }
                    catch (AggregateException)
                    {
                        this.state = null;
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex.GetType() + ": " + ex.Message);
                        this.state = null;
                    }
                } while (this.state != null);
            }
            finally
            {
#if NET45
                this.client.Close();
#else
                this.client.Dispose();
#endif
                this.Server.ConnectionClosed(this);    
            }
        }

        public async Task<String> ReadLineAsync()
        {
            var cancellationToken = new CancellationTokenSource();

            var t1 = this.streamReader.ReadLineAsync();
            var t2 = Task.Delay(60 * 1000, cancellationToken.Token);

            if (await Task.WhenAny(t1, t2) == t2)
                throw new TimeoutException();

            cancellationToken.Cancel();
            var line = t1.Result;

            this.Logger.LogRawSocketIn((line ?? "NULL"));
            if (line == null) throw new IOException("Line was null.");
            return line;
        }

        public async Task WriteLineAsync(String line)
        {
            this.Logger.LogRawSocketOut(line);
            var data = Encoding.UTF8.GetBytes(line + "\r\n");
            await (this.sslStream ?? (Stream)this.networkStream).WriteAsync(data, 0, data.Length);
        }

        public async Task<String> UpgradeConnectionAsync()
        {
            if (this.Server.X509Certificate == null)
                return "454 TLS not available due to temporary reason";

            this.sslStream = new SslStream(this.networkStream, true, null, null, EncryptionPolicy.RequireEncryption);
            await this.sslStream.AuthenticateAsServerAsync(this.Server.X509Certificate);

            if (this.streamReader != null)
                this.streamReader.Dispose();

            this.streamLimiter = new StreamLimiter(this.streamLimiter, 150 * 1024 * 1024);
            this.streamReader = new StreamReader(this.sslStream, Encoding.UTF8, false, 4069, true);
            this.IsSecureSocket = true;
            return null;
        }
    }
}
