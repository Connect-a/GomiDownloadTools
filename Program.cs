using Gomi.CLI;
using System.CommandLine;

var cmd = Builder.Build(Controller.Handle, new ParameterBinder());
await cmd.InvokeAsync(args);
