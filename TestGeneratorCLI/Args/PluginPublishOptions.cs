using CommandLine;

namespace TestGeneratorCLI.Args;

[Verb("plugin-publish", HelpText = "Сборка и публикация плагина TestGenerator")]
public class PluginPublishOptions
{
    [Option('d', "directory", Required = false, Default = null, 
        HelpText = "Директория проекта. По умолчанию используется текущая")]
    public string? Path { get; set; }
    
    [Option('t', "token", Required = true, 
        HelpText = "Токен")]
    public required string Token { get; set; }
    
    [Option('u', "url", Required = false, Default = null, 
        HelpText = "Ссылка на файл плагина во внешнем хранилище. Если указана, то будет загружена на сервер вместо файла.")]
    public string? Url { get; set; }
}