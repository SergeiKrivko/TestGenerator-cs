using AvaluxUI.Utils;
using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Shared.Types;

public abstract class BaseBuilder
{
    private IAppService _appService = Injector.Inject<IAppService>();

    public ISettingsSection Settings { get; }

    public Guid BuildId { get; }
    public IProject Project { get; }

    protected BaseBuilder(Guid id, IProject project, ISettingsSection settings)
    {
        BuildId = id;
        Settings = settings;
        Project = project;
    }

    public virtual Task<int> Compile(CancellationToken token = new()) => Task.FromResult(0);
    public virtual string? Command => null;

    [Obsolete("Используйте вариант с параметром 'environment'")]
    public virtual Task<ICompletedProcess> Run(string args = "", string? workingDirectory = null,
        string? stdin = null, CancellationToken token = new())
    {
        throw new NotImplementedException();
    }

    public virtual async Task<ICompletedProcess> Run(string args = "", string? workingDirectory = null,
        string? stdin = null, EnvironmentModel? environment = null, CancellationToken token = new())
    {
        try
        {
            return await Run(args, workingDirectory, stdin, token);
        }
        catch (NotImplementedException)
        {
            return await RunAsync(
                RunProcessArgs.ProcessRunProvider.Background, args, workingDirectory, stdin, environment,
                token: token);
        }
    }

    [Obsolete("Используйте вариант с параметром 'environment'")]
    public virtual Task<ICompletedProcess> RunConsole(string args = "",
        string? workingDirectory = null,
        string? stdin = null,
        CancellationToken token = new())
    {
        throw new NotImplementedException();
    }

    public virtual async Task<ICompletedProcess> RunConsole(string args = "",
        string? workingDirectory = null,
        string? stdin = null,
        EnvironmentModel? environment = null,
        CancellationToken token = new())
    {
        try
        {
            return await RunConsole(args, workingDirectory, stdin, token);
        }
        catch (NotImplementedException)
        {
            return await RunAsync(
                RunProcessArgs.ProcessRunProvider.RunTab, args, workingDirectory, stdin, environment,
                token: token);
        }
    }

    private async Task<ICompletedProcess> RunAsync(RunProcessArgs.ProcessRunProvider where, string args,
        string? workingDirectory = null,
        string? stdin = null,
        EnvironmentModel? environment = null,
        CancellationToken token = new())
    {
        if (Command == null)
            throw new Exception("Builder base runner: command is null");
        var lst = Command.Split();
        return await _appService.RunProcess(where, new RunProcessArgs
        {
            Filename = lst[0],
            Args = string.Join(' ', lst.Skip(1) + args),
            WorkingDirectory = workingDirectory,
            Stdin = stdin
        }, token: token);
    }

    public string TempPath => Path.Join(_appService.AppDataPath, "Temp", "Builds", BuildId.ToString());
}