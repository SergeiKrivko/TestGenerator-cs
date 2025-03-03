using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

public class Build : IBuild
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly BuildTypesService _buildTypesService = Injector.Inject<BuildTypesService>();

    public Guid Id { get; }

    public string Name
    {
        get => Settings.Get<string>("name") ?? "";
        set => Settings.Set("name", value);
    }

    public string WorkingDirectory
    {
        get => Settings.Get<string>("workingDirectory") ?? _project.Path;
        set => Settings.Set("workingDirectory", value);
    }

    public EnvironmentModel? Environment
    {
        get => Settings.Get<EnvironmentModel>("environment");
        set => Settings.Set("environment", value);
    }

    public List<BuildSubprocess> PreProc => Settings.Get<BuildSubprocess[]>("preProc", []).ToList();
    public List<BuildSubprocess> PostProc => Settings.Get<BuildSubprocess[]>("postProc", []).ToList();

    public string TypeName => Settings.Get<string>("type") ?? "";
    public BuildType Type { get; }
    public BaseBuilder Builder { get; }

    private readonly IProject _project;
    public SettingsFile Settings { get; }

    private Build(IProject project, Guid id)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));

        var type = Type = _buildTypesService.Get(Settings.Get<string>("type") ?? "");
        Builder = type.Builder(id, _project, Settings.GetSection("typeSettings"));
    }

    public delegate IBuild? BuildGetter(Guid id);

    public BuildGetter? GetBuild { get; set; }

    private Build(IProject project, Guid id, BuildType buildType)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));
        Type = buildType;
        Settings.Set("type", buildType.Key);
        var builder = Builder = buildType.Builder(id, _project, Settings.GetSection("typeSettings"));
        if (builder.GetType().GetMethod("Compile")?.Module.Name !=
            typeof(BaseBuilder).GetMethod("Compile")?.Module.Name)
        {
            var lst = Settings.Get<BuildSubprocess[]>("preProc", []).ToList();
            lst.Add(new BuildSubprocess { Compile = true });
            Settings.Set("preProc", lst);
        }
    }

    public static Build FromFile(string path)
    {
        var filename = Path.GetFileNameWithoutExtension(path);
        return new Build(Injector.Inject<ProjectsService>().Current, Guid.Parse(filename));
    }

    public static Build Load(Guid id)
    {
        return new Build(Injector.Inject<ProjectsService>().Current, id);
    }

    public static Build New(BuildType buildType)
    {
        return new Build(Injector.Inject<ProjectsService>().Current, Guid.NewGuid(), buildType);
    }

    public string? Command => Builder.Command;

    public Task<int> Compile(CancellationToken token = new()) => Builder.Compile(token: token);

    public Task<ICompletedProcess>
        Run(string args = "", string? stdin = null, CancellationToken token = new()) =>
        Builder.Run(args, WorkingDirectory, stdin, Environment, token: token);

    public Task<ICompletedProcess> RunConsole(string args = "", string? stdin = null,
        CancellationToken token = new()) =>
        Builder.RunConsole(args, WorkingDirectory, stdin, Environment, token: token);

    private IBuild? _getBuild(Guid id)
    {
        if (GetBuild == null)
            throw new Exception("Can not get build");
        return GetBuild(id);
    }

    private async Task<int> RunSubProcConsole(List<BuildSubprocess> procs, CancellationToken token = new())
    {
        var code = 0;
        foreach (var proc in procs)
        {
            if (proc.Compile)
            {
                code = await _appService
                    .RunBackgroundTask($"{Name} - компиляция", Compile, BackgroundTaskFlags.CanBeCancelled).Wait();
            }
            else if (proc.Command != null)
            {
                var lst = proc.Command.Split();
                code = (await _appService
                    .RunProcess(RunProcessArgs.ProcessRunProvider.RunTab,
                        new RunProcessArgs
                        {
                            Filename = lst[0],
                            Args = string.Join(' ', lst.Skip(1)),
                            WorkingDirectory = WorkingDirectory
                        }, token: token)).ExitCode;
            }
            else if (proc.BuildId != null)
            {
                var build = _getBuild(proc.BuildId.Value);
                if (build != null)
                    code = await build.ExecuteConsole(token: token);
            }

            if (code != 0)
                return code;
        }

        return code;
    }

    private async Task<int> RunSubProc(List<BuildSubprocess> procs, CancellationToken token = new())
    {
        var code = 0;
        foreach (var proc in procs)
        {
            if (proc.Compile)
            {
                code = await _appService
                    .RunBackgroundTask($"{Name} - компиляция", Compile, BackgroundTaskFlags.CanBeCancelled).Wait();
            }
            else if (proc.Command != null)
            {
                var lst = proc.Command.Split();
                code = (await _appService
                    .RunProcess(new RunProcessArgs
                    {
                        Filename = lst[0],
                        Args = string.Join(' ', lst.Skip(1)),
                        WorkingDirectory = WorkingDirectory
                    }, token)).ExitCode;
            }
            else if (proc.BuildId != null)
            {
                var build = _getBuild(proc.BuildId.Value);
                if (build != null)
                    code = await build.Execute(token: token);
            }

            if (code != 0)
                return code;
        }

        return code;
    }

    public async Task<int> RunPreProcConsole(CancellationToken token = new())
    {
        return await RunSubProcConsole(PreProc, token: token);
    }

    public async Task<int> RunPostProcConsole(CancellationToken token = new())
    {
        return await RunSubProcConsole(PostProc, token: token);
    }

    public async Task<int> RunPreProc(CancellationToken token = new())
    {
        return await RunSubProc(PreProc, token: token);
    }

    public async Task<int> RunPostProc(CancellationToken token = new())
    {
        return await RunSubProc(PostProc, token: token);
    }

    public async Task<int> ExecuteConsole(string args = "", string? stdin = null,
        CancellationToken token = new())
    {
        var code = await RunPreProcConsole(token);
        if (code != 0)
            return code;
        code = (await RunConsole(args, stdin, token: token)).ExitCode;
        if (code != 0)
            return code;
        return await RunPostProcConsole(token);
    }

    public async Task<int> Execute(string args = "", string? stdin = null, CancellationToken token = new())
    {
        var code = await RunPreProc(token);
        if (code != 0)
            return code;
        code = (await Run(args, stdin, token: token)).ExitCode;
        if (code != 0)
            return code;
        return await RunPostProc(token);
    }
}