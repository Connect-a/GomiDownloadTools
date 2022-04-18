using System.Collections.Concurrent;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Gomi.Core;

public class Downloader
{
  private readonly ILogger _logger;
  private readonly HttpClient _client;
  private readonly DownloadOptions _options;
  private int _downloadCount = 0;
  private int _existsCount = 0;
  private readonly HashSet<string> _downloadedFiles = new();
  private readonly ConcurrentDictionary<string, SocketError> _socketErrorMap = new();

  public Downloader(ILoggerFactory loggerFactory, DownloadOptions options, HttpClient client)
  {
    _logger = loggerFactory.CreateLogger<Downloader>();
    _client = client;
    _options = options;
  }

  public Task ExecuteAsync(IEnumerable<Uri> uris) =>
    Parallel.ForEachAsync(uris, async (uri, token) =>
    {
      _logger.LogInformation($"Downloading > {uri.AbsoluteUri}");
      var filePath = FilePathConstructor.ConvertUriToFilePath(_options.Destination, uri);
      if (File.Exists(filePath))
      {
        _existsCount++;
        return;
      }
      if (_downloadedFiles.Contains(filePath)) return;
      _downloadedFiles.Add(filePath);

      // Check socket connection status.
      // HACK: For reduce the number of SocketException.
      var target = uri.GetLeftPart(UriPartial.Authority);
      {
        var r = _socketErrorMap.TryGetValue(target, out var socketError);
        if (r && socketError != SocketError.Success) return;
      }

      // Donwload
      var res = default(HttpResponseMessage);
      try
      {
        res = await _client.GetAsync(uri, token);
      }
      catch (HttpRequestException e) when (e.InnerException is SocketException ex)
      {
        _socketErrorMap.AddOrUpdate(target, ex.SocketErrorCode, (_, _) => ex.SocketErrorCode);
        _logger.LogWarning(ex, $"Download failed > {uri.AbsoluteUri}");
        return;
      }
      _socketErrorMap.AddOrUpdate(target, SocketError.Success, (_, _) => SocketError.Success);

      if (res.IsSuccessStatusCode)
      {
        _downloadCount++;
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
        using var file = File.Create(filePath);
        await res.Content.CopyToAsync(file, token);
      }
      else _logger.LogWarning($"Download failed [{res.StatusCode}] > {uri.AbsoluteUri}");

      return;
    });
}