using TestGenerator.Core.Models;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Core.Services;

public class StartupService
{
    public static StartupInfoModel? StartupInfo { get; set; } = null;
    
    public static StartupInfoModel ParseArgs(string[] args)
    {
        if (args.Length >= 2 && args[0] == "-d")
        {
            return new StartupInfoModel { Directory = args[1], Files = args.Skip(2).ToArray()};
        }

        return new StartupInfoModel { Files = args };
    }
    
    public SettingsFile Settings { get; } = SettingsFile.Open(Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko",
        Config.AppName, "Settings.xml"));
}