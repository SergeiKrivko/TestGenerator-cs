using CommandLine;

namespace TestGenerator.PluginInstaller.Args;

[Verb("clear", HelpText = "Очистка неиспользуемых/сломанных плагинов")]
public class ClearOptions : BaseOptions
{
    [Value(0, Required = true, HelpText = "ID сломанных плагинов")]
    public Guid[] BrokenInstallationIds { get; set; } = [];
}