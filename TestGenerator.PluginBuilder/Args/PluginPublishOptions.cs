using CommandLine;

namespace TestGenerator.PluginBuilder.Args;

[Verb("publish", HelpText = "Сборка и публикация плагина TestGenerator")]
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
    
    [Option("github", Required = false, Default = false, 
        HelpText = "Опубликовать плагин в релизах на Github. Требует github-user, github-repo и github-token")]
    public bool Github { get; set; }
    
    [Option("github-user", Required = false, Default = null, 
        HelpText = "Имя пользователя Github.")]
    public string? GithubUser { get; set; }
    
    [Option("github-repo", Required = false, Default = null, 
        HelpText = "Название репозитория Github.")]
    public string? GithubRepo { get; set; }
    
    [Option("github-token", Required = false, Default = null, 
        HelpText = "Токен Github. Требуются права на создание релизов в указанном выше репозитории")]
    public string? GithubToken { get; set; }
    
    [Option('r', "runtime", Required = false, Default = null, 
        HelpText = "Идентификатор среды выполнения. Имеет значение, только если в  Config.json указано \"PlatformSpecific\": true. " +
                   "По умолчанию используется текущая среда")]
    public string? Runtime { get; set; }
}