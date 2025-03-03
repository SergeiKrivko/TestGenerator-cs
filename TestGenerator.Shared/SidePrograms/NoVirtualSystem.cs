using AvaluxUI.Utils;
using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

internal class NoVirtualSystem : IVirtualSystem
{
    public string Key => "";
    public string Name => "";
    
    private IAppService _appService = Injector.Inject<IAppService>();

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

    public async Task<ICompletedProcess> Execute(RunProcessArgs.ProcessRunProvider where, RunProcessArgs args, CancellationToken token = new())
    {
        return await _appService.RunProcess(where, args, token);
    }

    public async Task<ICompletedProcess> Execute(RunProcessArgs args, CancellationToken token = new())
    {
        return await _appService.RunProcess(args, token);
    }

    public async Task<ICollection<ICompletedProcess>> Execute(RunProcessArgs.ProcessRunProvider where, RunProcessArgs[] args, CancellationToken token = new())
    {
        return await _appService.RunProcess(where, args, token);
    }

    public async Task<ICollection<ICompletedProcess>> Execute(RunProcessArgs[] args, CancellationToken token = new())
    {
        return await _appService.RunProcess(args, token);
    }

    public Task<string> ConvertPath(string path)
    {
        return Task.FromResult(path);
    }
}