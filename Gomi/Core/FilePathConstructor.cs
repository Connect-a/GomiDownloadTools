using System.Text;
using System.Web;

namespace Gomi.Core;

public static class FilePathConstructor
{
  private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars().Where(x => x != '/' && x != '\\').ToArray();
  private static readonly IReadOnlyDictionary<string, string> _invalidFileNameStrings = _invalidFileNameChars.ToDictionary(x => $"{x}", x => HttpUtility.UrlEncode(Encoding.UTF8.GetBytes($"{x}")));

  public static string ConvertUriToFilePath(DirectoryInfo dest, Uri uri)
  {
    var path = HttpUtility.UrlDecode($"{uri.Authority}{Path.GetDirectoryName(uri.AbsolutePath)}".Trim('\\'));
    if (path.IndexOfAny(_invalidFileNameChars) != -1)
    {
      foreach (var c in _invalidFileNameStrings) path = path.Replace(c.Key, c.Value);
    }
    var dir = $"{dest.FullName}\\{path}";

    var fileName = HttpUtility.UrlDecode(Path.GetFileName(uri.AbsolutePath));
    if (string.IsNullOrEmpty(fileName)) fileName = "index.html";
    if (fileName.IndexOfAny(_invalidFileNameChars) != -1)
    {
      foreach (var c in _invalidFileNameStrings) fileName = fileName.Replace(c.Key, c.Value);
    }

    return $"{dir}\\{fileName}";
  }
}