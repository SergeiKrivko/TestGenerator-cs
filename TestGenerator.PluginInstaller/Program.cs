using System.Reflection;
using CommandLine;
using TestGenerator.PluginInstaller;
using TestGenerator.PluginInstaller.Args;

var types = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();

var service = new InstallService();
service.Initialize();

await Parser.Default.ParseArguments(args, types)
    .WithParsedAsync(Run);

return 0;


async Task Run(object arg)
{
    await Task.Delay(200);

    try
    {
        switch (arg)
        {
            case InstallPluginOptions options:
                service.Install(options.ZipPath, options.NewInstallationId);
                break;
            case RemovePluginOptions options:
                service.Remove(options.OldInstallationId);
                break;
            case UpdatePluginOptions options:
                service.Update(options.OldInstallationId, options.ZipPath, options.NewInstallationId);
                break;
        }

        if (arg is BaseOptions baseOptions)
        {
            if (baseOptions.ClearDeleted)
                service.ClearDeleted();
            if (baseOptions.ClearDuplicates)
                service.ClearDuplicates();
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}