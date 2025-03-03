using Avalonia;
using System;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;
using AvaluxUI.Utils;

namespace TestGenerator;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        LogService.Init();
        var startupInfo = StartupService.ParseArgs(args);
        if (!startupInfo.StartGui)
            return;
        StartupService.StartupInfo = startupInfo;

        Injector.AddService<AppService>();
        Injector.AddService<PluginsService>();
        Injector.AddService<ProjectTypesService>();
        Injector.AddService<ProjectsService>();
        Injector.AddService<BuildTypesService>();
        Injector.AddService<BuildsService>();
        Injector.AddService<PluginsHttpService>();

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogService.Logger.Fatal($"{e.GetType()}: {e.Message}");
#if DEBUG
            throw;
#else
            Process.Start(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "TestGenerator.ErrorHandler"), LogService.LogFilePath);
#endif
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}