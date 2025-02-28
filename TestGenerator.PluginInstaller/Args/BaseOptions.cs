using CommandLine;

namespace TestGenerator.PluginInstaller.Args;

public class BaseOptions
{
    [Option("clear-deleted", HelpText = "Удаляет все плагины, помеченные как IsDeleted")]
    public bool ClearDeleted { get; set; }

    [Option("clear-duplicates", HelpText = "Удаляет дубликаты плагинов. Оставляет самую новую версию")]
    public bool ClearDuplicates { get; set; }
}