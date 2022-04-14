using System.Text.Json.Nodes;
using Gomi.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gomi.CLI
{
  public static class Controller
  {
    public static Func<Parameter, Task> Handle = (p) =>
    {
      // ConfigureServices
      // NOTE: Move if built-in DI is introduced in System.CommandLine.
      //  https://github.com/dotnet/command-line-api/issues?q=label%3A%22Area-Hosting+and+DI%22
      var services = new ServiceCollection();
      var logLevel = LogLevel.Warning;
      if (p.Verbose) logLevel = LogLevel.Information;
      if (p.Silent) logLevel = LogLevel.None;
      services
        .AddHttpClient()
        .AddScoped<Gomi.Core.Downloader>()
        .AddScoped<Gomi.TemplateDownload.Downloader>()
        .AddScoped<Gomi.Har.Downloader>()
        .AddScoped<Gomi.Har.Extractor>()
        .AddLogging(l => l.AddSimpleConsole().SetMinimumLevel(logLevel))
        .AddScoped<DownloadOptions>(_ => new() { Destination = p.Destination });
      var serviceProvider = services.BuildServiceProvider();

      // 
      if (p.Template is not null && p.Csv is not null)
      {
        var downloader = serviceProvider.GetRequiredService<Gomi.TemplateDownload.Downloader>();
        return downloader.ExecuteAsync(p.Template, p.Csv);
      }

      if (p.Har?.Any() ?? false)
      {
        var extractor = serviceProvider.GetRequiredService<Gomi.Har.Extractor>();
        var downloader = serviceProvider.GetRequiredService<Gomi.Har.Downloader>();
        return Task.Run(async () =>
        {
          var parsedHar = p.Har.Select(x => JsonObject.Parse(File.ReadAllText(x.FullName))).ToArray();
          await Task.WhenAll(parsedHar.Select(extractor.ExecuteAsync));
          await Task.WhenAll(parsedHar.Select(downloader.ExecuteAsync));
        });
      }

      return Task.CompletedTask;
    };
  }
}