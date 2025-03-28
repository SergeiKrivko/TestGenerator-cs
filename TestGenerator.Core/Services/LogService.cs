﻿using Serilog;
using Serilog.Events;
using Logger = TestGenerator.Shared.Utils.Logger;

namespace TestGenerator.Core.Services;

public class LogService
{
    public static string LogFilePath { get; } = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko", Config.AppName,
        "Logs", $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.txt");

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
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel:
#if DEBUG
                LogEventLevel.Debug
#else
                LogEventLevel.Warning
#endif
            )
            .WriteTo.File(LogFilePath,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static Logger GetLogger(string name) => new Logger(name, Logger);
}