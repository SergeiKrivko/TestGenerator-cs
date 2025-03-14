﻿using CommandLine;

namespace TestGenerator.PluginBuilder.Args;

[Verb("build", HelpText = "Сборка плагина TestGenerator")]
public class PluginBuildOptions
{
    [Option('d', "directory", Required = false, Default = null,
        HelpText = "Директория проекта. По умолчанию используется текущая")]
    public string? Path { get; set; }

    [Option('o', "output", Required = false, Default = null,
        HelpText = "Имя выходного файла. По умолчанию <название плагина>.zip")]
    public string? Output { get; set; }

    [Option('i', "install", Required = false, Default = false,
        HelpText = "Установить плагин после сборки")]
    public bool Install { get; set; }

    [Option('r', "runtime", Required = false, Default = null,
        HelpText =
            "Идентификатор среды выполнения. Имеет значение, только если в  Config.json указано \"PlatformSpecific\": true. " +
            "По умолчанию используется текущая среда")]
    public string? Runtime { get; set; }
}