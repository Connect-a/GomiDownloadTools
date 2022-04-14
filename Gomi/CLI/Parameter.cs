namespace Gomi.CLI;

public class Parameter
{
  public FileInfo? Template { get; init; }
  public FileInfo? Csv { get; init; }
  public IEnumerable<FileInfo>? Har { get; init; }
  public bool Flatten { get; init; }
  public bool Silent { get; init; }
  public bool Verbose { get; init; }
  public IEnumerable<FileInfo>? Files { get; init; }
  public IEnumerable<FileInfo>? UrlFiles { get; init; }
  public DirectoryInfo Destination { get; init; } = default!;
}