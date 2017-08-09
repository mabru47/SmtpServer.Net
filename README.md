# SmtpServer.Net
SmtpServer.Net is a simple and high performant open source SMTP-Server for .Net Standard >=1.4 and .Net >=4.5.

## Usage
The following example instantiates a new `SmtpServer` and adds some an example plugin.

``` csharp
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
``` 

## Example Plugin

``` csharp
class ExampleMailProcessor : IMailProcessor
{
    public async Task<MailProcessorResult> ProcessAsync(string from, string[] rcpt, Stream data)
    {
        var stringData = await new StreamReader(data).ReadToEndAsync();
        Console.WriteLine("Total length: " + stringData.Length);

        return new MailProcessorResult();
    }
}
``` 

## Builds

Get it via NuGet https://www.nuget.org/packages/Tireless.SmtpServer.Net/

```
PM> Install-Package Tireless.SmtpServer.Net
```

### Build from Code
Just clone the repository and open the solution in Visual Studio 2017.
Or use the dotnet client via command line.
