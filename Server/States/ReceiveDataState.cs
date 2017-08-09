using Tireless.Net.Mail.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.States
{
    class ReceiveDataState : StateBase
    {
        private String host;
        private String mailFrom;
        private Int32? mailSize;
        private List<String> mailTo;
        private Guid mailGuid;

        public ReceiveDataState(String host, String mailFrom, Int32? mailSize, List<String> mailTo, DataCommand cmd, Connection connection) : base(connection)
        {
            this.host = host;
            this.mailFrom = mailFrom;
            this.mailSize = mailSize;
            this.mailTo = mailTo;
            this.mailGuid = Guid.NewGuid();
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            await base.Connection.WriteLineAsync("354 Start mail input; end with <CRLF>.<CRLF>");

            Byte[] endOfMail = Encoding.ASCII.GetBytes("\r\n.\r\n");

            Stream streamBuffer;

            if (this.mailSize != null && this.mailSize < 1024 * 1024)
                streamBuffer = new MemoryStream(this.mailSize.Value);
            else
            {
                if (Directory.Exists(this.Server.Settings.TempPath) == false)
                    Directory.CreateDirectory(this.Server.Settings.TempPath);

                var file = this.Server.Settings.TempPath + this.mailGuid + ".txt";
                streamBuffer = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose);
            }

            using (var streamWriter = new StreamWriter(streamBuffer, Encoding.UTF8, 4096, true)
            {
                NewLine = "\r\n"
            })
            {
                streamWriter.WriteLine("Received: from " + this.host + " (" + this.Connection.RemoteEndPoint + ")");
                streamWriter.WriteLine(" by " + this.Server.Settings.ServiceDomain + " (" + this.Connection.LocalEndPoint + ") with instance id [" + this.Server.Settings.InstanceID + "]");
                streamWriter.WriteLine(" for <" + String.Join(";", this.mailTo) + "> ");
                streamWriter.WriteLine(" queued as [" + this.mailGuid + "];" + DateTime.UtcNow.ToString("r"));


                String line;
                while ((line = await base.Connection.ReadLineAsync()) != ".")
                {
                    if (line.StartsWith(".."))
                        line = line.Substring(1);

                    await streamWriter.WriteLineAsync(line);
                }
                await streamWriter.FlushAsync();
            }
            streamBuffer.Position = 0;
            return new DataCompleteState(this.host, this.mailFrom, this.mailTo, this.mailGuid, streamBuffer, this.Connection);
        }
    }
}
