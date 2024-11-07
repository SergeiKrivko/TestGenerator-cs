using Serilog;
using Logger = TestGenerator.Shared.Utils.Logger;

namespace TestGenerator.Core.Services;

public class LogService
{
    public static string LogFilePath { get; } = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko", Config.AppName,
        "log.txt");

    public static ILogger Logger { get; private set; } = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    public static void Init()
    {
        if (File.Exists(LogFilePath))
            File.Delete(LogFilePath);
        Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(LogFilePath, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static Logger GetLogger(string name) => new Logger(name, Logger);
}