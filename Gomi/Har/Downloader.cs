using System.Text.Json.Nodes;

namespace Gomi.Har;

public class Downloader
{
  private readonly Gomi.Core.Downloader _downloader;
  public Downloader(Gomi.Core.Downloader downloader)
  {
    _downloader = downloader;
  }

  public Task ExecuteAsync(JsonNode? data)
  {
    if (data is null) return Task.CompletedTask;

    var uris = (data["log"]?["entries"]?.AsArray() ?? new JsonArray())
      .Select(entry =>
      {
        if (entry is null) return default;
        var req = entry["request"];
        if (req is null) return default;

        if (req["method"]?.GetValue<string>().ToLower() != "get") return default;
        var r = Uri.TryCreate(req["url"]?.GetValue<string>() ?? "", UriKind.Absolute, out var url);
        if (!r) return default;
        return url;
      }).OfType<Uri>();

    return _downloader.ExecuteAsync(uris);
  }

  public Task ExecuteAsync(FileInfo harFile) =>
    ExecuteAsync(JsonObject.Parse(File.ReadAllText(harFile.FullName)));
}