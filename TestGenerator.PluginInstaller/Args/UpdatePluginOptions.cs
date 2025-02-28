using CommandLine;

namespace TestGenerator.PluginInstaller.Args;

[Verb("update", HelpText = "Обновление плагина TestGenerator")]
public class UpdatePluginOptions : BaseOptions
{
    [Value(0, Required = true, HelpText = "Старый ID")]
    public Guid OldInstallationId { get; set; }

    [Value(1, Required = true, HelpText = "Путь к новому плагину")]
    public string ZipPath { get; set; } = string.Empty;

    [Value(2, Required = false, Default = null, HelpText = "Новый ID")]
    public Guid? NewInstallationId { get; set; }
}