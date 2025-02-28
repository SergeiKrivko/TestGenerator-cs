using CommandLine;

namespace TestGenerator.PluginInstaller.Args;

[Verb("install", HelpText = "Установка плагина TestGenerator")]
public class InstallPluginOptions : BaseOptions
{
    [Value(0, Required = true, HelpText = "Путь к новому плагину")]
    public string ZipPath { get; set; } = string.Empty;

    [Value(1, Required = false, Default = null, HelpText = "Новый ID")]
    public Guid? NewInstallationId { get; set; }
}