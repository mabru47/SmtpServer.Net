using System;
using System.Net;
using Tireless.Net.Mail;
using Tireless.Net.Mail.Models;
using Tireless.Net.Mail.Plugins;
using Tireless.Net.Mail.Plugins.Spamhaus;
using System.Threading.Tasks;
using System.IO;

namespace SmtpServerExample
{
    class ExampleMailProcessor : IMailProcessor
    {
        public async Task<MailProcessorResult> ProcessAsync(string from, string[] rcpt, Guid mailGuid, Stream data)
        {
            var stringData = await new StreamReader(data).ReadToEndAsync();
            Console.WriteLine("Total length: " + stringData.Length);

            return new MailProcessorResult();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(new ServerSettings()
            {
                Endpoint = IPAddress.Parse("127.0.0.1"),
                ServiceName = "ExampleService",
                ServiceDomain = "mx.example.org"
            });

            server.AddPluginAsync(new SpamhausPlugin()).Wait();
            server.AddPluginAsync(new ExampleMailProcessor()).Wait();

            server.Run();
        }
    }
}