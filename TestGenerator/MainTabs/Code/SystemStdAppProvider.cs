using System;
using System.Diagnostics;
using System.IO;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public class SystemStdAppProvider : IEditorProvider
{
    public string Key => "SystemStdApp";
    public string Name => "Приложение по умолчанию";
    public string[]? Extensions => null;
    public int Priority => 1;

    public OpenedFile? Open(string path)
    {
        Console.WriteLine($"Starting '{path}'");
        if (OperatingSystem.IsWindows())
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(path),
                Verb = "OPEN"
            });
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start(new ProcessStartInfo { FileName = "xdg-open", Arguments = path, UseShellExecute = true });
        }
        else if (OperatingSystem.IsMacOS())
        {
            Process.Start(new ProcessStartInfo { FileName = "open", Arguments = path, UseShellExecute = true });
        }
        else
        {
            LogService.Logger.Error("Can not open file on this operating system");
        }

        return null;
    }
}