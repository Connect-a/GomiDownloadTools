namespace Gomi.Core;

public class DownloadOptions
{
  public DirectoryInfo Destination { get; init; } = default!;
  public bool Silent { get; init; } = false;
}