using System.Text;
using System.Text.Json.Nodes;
using Gomi.Core;

namespace Gomi.Har
{
  public class Extractor
  {
    private readonly DownloadOptions _options;
    private int _existsCount = 0;

    public Extractor(DownloadOptions options)
    {
      _options = options;
    }

    public async Task ExecuteAsync(JsonNode? data)
    {
      if (data is null) return;

      foreach (var entry in data["log"]?["entries"]?.AsArray() ?? new JsonArray())
      {
        if (entry is null) continue;
        var req = entry["request"];
        if (req is null) continue;
        if ((req["method"]?.GetValue<string>().ToLower() ?? "") != "get") continue;
        var url = req["url"];
        if (url is null) continue;
        var resp = entry["response"];
        if (resp is null) continue;
        var content = resp["content"];
        if (content is null) continue;
        var text = content["text"];
        if (text is null) continue;

        // Construct FilePath
        var uri = new Uri(url.GetValue<string>());
        var fileName = Path.GetFileName(uri.AbsolutePath);
        if (string.IsNullOrEmpty(fileName)) fileName = "index.html";
        var dir = $"{_options.Destination.FullName}/{uri.Authority}/{Path.GetDirectoryName(uri.AbsolutePath)}/".Replace("//", "/");
        var filePath = $"{dir}{fileName}";
        if (File.Exists(filePath))
        {
          _existsCount++;
          continue;
        }

        // Extract
        var encoding = content["encoding"];
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
        using var file = File.Create(filePath);
        if ((encoding?.GetValue<string>().ToLower() ?? "") == "base64")
        {
          await file.WriteAsync(Convert.FromBase64String(text.GetValue<string>()));
        }
        else
        {
          await file.WriteAsync(Encoding.UTF8.GetBytes(text.GetValue<string>()));
        }
      }

    }

    public Task ExecuteAsync(FileInfo harFile) =>
      ExecuteAsync(JsonObject.Parse(File.ReadAllText(harFile.FullName)));
  }
}