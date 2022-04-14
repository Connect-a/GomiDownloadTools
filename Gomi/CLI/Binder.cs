using System.CommandLine.Binding;

namespace Gomi.CLI;

/// <inheritdoc/>
public class ParameterBinder : BinderBase<Parameter>
{
  /// <inheritdoc/>
  protected override Parameter GetBoundValue(BindingContext ctx)
  {
    var template = ctx.ParseResult.GetValueForOption(CommandArgs.TemplateOption);
    var csv = ctx.ParseResult.GetValueForOption(CommandArgs.CsvOption);
    var har = ctx.ParseResult.GetValueForOption(CommandArgs.HarOption) ?? Enumerable.Empty<FileInfo>();
    var list = ctx.ParseResult.GetValueForOption(CommandArgs.ListOption) ?? Enumerable.Empty<FileInfo>();
    var files = ctx.ParseResult.GetValueForArgument(CommandArgs.FilesArgument) ?? Enumerable.Empty<FileInfo>();

    var harList = har.ToList();
    var listList = list.ToList();
    var fileList = files.ToList();
    if (template is null && csv is null && !harList.Any() && !listList.Any() && !fileList.Any())
    {
      throw new ArgumentException("no parameter");
    }

    // File existing
    if (files is not null)
    {
      foreach (var file in files)
      {
        if (!file.Exists) throw new FileNotFoundException("File not found", file.FullName);
        if (file.FullName.EndsWith(".har")) harList.Add(file);
        if (file.FullName.EndsWith(".txt")) listList.Add(file);
        if (file.FullName.EndsWith(".csv")) listList.Add(file);
      }

      if (template is null) template = files.FirstOrDefault(x => x.Extension == ".txt");
      if (csv is null) csv = files.FirstOrDefault(x => x.Extension == ".csv");
    }

    if (harList.Any(x => !x.Exists)) throw new FileNotFoundException("File not found", harList.First(x => !x.Exists).FullName);
    if (listList.Any(x => !x.Exists)) throw new FileNotFoundException("File not found", listList.First(x => !x.Exists).FullName);
    if (template is not null && csv is not null)
    {
      if (!template.Exists) throw new FileNotFoundException("File not found", template.FullName);
      if (!csv.Exists) throw new FileNotFoundException("File not found", csv.FullName);
    }

    // Setup destination folder
    var destination = ctx.ParseResult.GetValueForOption(CommandArgs.OutputOption);
    if (destination is null)
    {
      var dirName = $"dest_{DateTime.Now:yyyyMMdd_HHmm}";
      if (template is not null) dirName = $"{template.FullName}.dl";
      if (harList.Any())
      {
        if (harList.Count() == 1) dirName = $"{harList.First().FullName}.dl";
        if (harList.Count() > 1) dirName = $"hars.dl";
      }
      destination = new DirectoryInfo(dirName);
    }
    // NOTE: IOExceptions may occur.
    destination.Create();

    return new Parameter
    {
      Template = template,
      Csv = csv,
      Har = harList,
      Flatten = ctx.ParseResult.GetValueForOption(CommandArgs.FlattenOption),
      Silent = ctx.ParseResult.GetValueForOption(CommandArgs.SilentOption),
      Verbose = ctx.ParseResult.GetValueForOption(CommandArgs.VerboseOption),
      Files = fileList,
      UrlFiles = listList,
      Destination = destination,
    };
  }
}