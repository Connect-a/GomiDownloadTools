using System.CommandLine;
using System.CommandLine.Binding;

namespace Gomi.CLI;

public static class CommandArgs
{
  public static readonly Option<FileInfo?> TemplateOption = new(new string[] { "--template", "-t" }, "Download template file");
  public static readonly Option<FileInfo?> CsvOption = new(new string[] { "--csv", "-c" }, "CSV file corresponding to the template file");
  public static readonly Option<FileInfo[]?> HarOption = new(new string[] { "--har", "-h" }, "HAR files");
  public static readonly Option<FileInfo[]?> ListOption = new(new string[] { "--list", "-l" }, "URL list files");
  public static readonly Option<DirectoryInfo?> OutputOption = new(new string[] { "--output", "-o" }, "Download directory");
  public static readonly Option<bool> FlattenOption = new(new string[] { "--flatten", "-f" }, "Download with flatten");
  public static readonly Option<bool> SilentOption = new(new string[] { "--silent", "-s" }, "Disables diagnostics output");
  public static readonly Option<bool> VerboseOption = new(new string[] { "--verbose", "-v" }, "Displays verbose output");
  public static readonly Argument<FileInfo[]?> FilesArgument = new("Files Input.");

  public static readonly IEnumerable<IValueDescriptor> List = new List<IValueDescriptor>(){
    TemplateOption,
    CsvOption,
    HarOption,
    ListOption,
    OutputOption,
    FlattenOption,
    SilentOption,
    VerboseOption,
    FilesArgument
  };
}