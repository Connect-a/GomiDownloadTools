using System.CommandLine;
using System.CommandLine.Binding;

namespace Gomi.CLI
{
  public static class Builder
  {
    public static Command Build<T>(Func<T, Task> handle, BinderBase<T> binder)
    {
      var cmd = new RootCommand("Download Toolset");
      foreach (var p in CommandArgs.List)
      {
        if (p is Option o) cmd.Add(o);
        if (p is Argument a) cmd.Add(a);
      }

      cmd.SetHandler(handle, binder);
      return cmd;
    }
  }
}