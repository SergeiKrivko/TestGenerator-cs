using Avalonia;
using System;
using Avalonia.ReactiveUI;
using TestGenerator.Core.Services;

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
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogService.Logger.Fatal($"{e.GetType()}: {e.Message}");
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}