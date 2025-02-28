using CommandLine;

namespace TestGenerator.PluginInstaller.Args;

[Verb("remove", HelpText = "Удаление плагина TestGenerator")]
public class RemovePluginOptions : BaseOptions
{
    [Value(0, Required = true, HelpText = "Старый ID")]
    public Guid OldInstallationId { get; set; }
}