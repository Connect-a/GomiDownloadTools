using Microsoft.Extensions.Logging;

namespace Gomi.TemplateDownload
{
  public class Downloader
  {
    private readonly ILogger _logger;
    private readonly Gomi.Core.Downloader _downloader;
    public Downloader(ILoggerFactory loggerFactory, Gomi.Core.Downloader downloader)
    {
      _logger = loggerFactory.CreateLogger<Downloader>();
      _downloader = downloader;
    }

    public Task ExecuteAsync(FileInfo templateFile, FileInfo csvFile)
    {
      var csvHeader = (File.ReadLines(csvFile.FullName).FirstOrDefault() ?? "").Split(",");
      if (!csvHeader.Any()) throw new ArgumentException("Invalid CSV input.");

      var template = File.ReadAllLines(templateFile.FullName);
      var uris = File
        .ReadLines(csvFile.FullName)
        .Skip(1)
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .SelectMany(row =>
        {
          var values = row.Split(",");
          return template.Select(t =>
          {
            var url = t;
            for (int i = 0; i < csvHeader.Length; i++)
            {
              url = url.Replace($"{{{csvHeader[i]}}}", values[i]);
            }
            var r = Uri.TryCreate(url, UriKind.Absolute, out var uri);
            if (r) return uri;
            else
            {
              _logger.LogWarning($"Illegal URL > {url}");
              return default;
            }
          }).OfType<Uri>();
        });

      return _downloader.ExecuteAsync(uris);
    }
  }
}