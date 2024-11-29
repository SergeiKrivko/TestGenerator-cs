using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

internal class NoVirtualSystem : IVirtualSystem
{
    public string Key => "";
    public string Name => "";

    public ICollection<string> Tags
    {
        get
        {
            List<string> res = ["Default"];
            if (OperatingSystem.IsWindows())
                res.Insert(0, "Windows");
            else if (OperatingSystem.IsLinux())
                res.Insert(0, "Linux");
            else if (OperatingSystem.IsMacOS())
                res.Insert(0, "MacOS");
            return res;
        }
    }
    public async Task<ICompletedProcess> Execute(string filename, string args)
    {
        return await AAppService.Instance.RunProcess(filename, args);
    }

    public async Task<ICompletedProcess> Execute(string command)
    {
        return await AAppService.Instance.RunProcess(command);
    }

    public ITerminalController ExecuteInConsole(string command, string? workingDirectory = null)
    {
        return AAppService.Instance.RunInConsole(command, workingDirectory);
    }

    public async Task<string> ConvertPath(string path)
    {
        return path;
    }
}