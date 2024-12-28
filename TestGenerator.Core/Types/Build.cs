using TestGenerator.Core.Services;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Core.Types;

public class Build : ABuild
{
    public override Guid Id { get; }

    public override string Name
    {
        get => Settings.Get<string>("name") ?? "";
        set => Settings.Set("name", value);
    }

    public override string WorkingDirectory
    {
        get => Settings.Get<string>("workingDirectory") ?? _project.Path;
        set => Settings.Set("workingDirectory", value);
    }

    public override EnvironmentModel? Environment
    {
        get => Settings.Get<EnvironmentModel>("environment");
        set => Settings.Set("environment", value);
    }

    public override List<BuildSubprocess> PreProc => Settings.Get<BuildSubprocess[]>("preProc", []).ToList();
    public override List<BuildSubprocess> PostProc => Settings.Get<BuildSubprocess[]>("postProc", []).ToList();

    public override string TypeName => Settings.Get<string>("type") ?? "";
    public override BuildType Type { get; }
    public override BaseBuilder Builder { get; }

    private readonly Project _project;
    public SettingsFile Settings { get; }

    private Build(Project project, Guid id)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));

        var type = Type = BuildTypesService.Instance.Get(Settings.Get<string>("type") ?? "");
        Builder = type.Builder(id, _project, Settings.GetSection("typeSettings"));
    }

    public delegate ABuild? BuildGetter(Guid id);

    public BuildGetter? GetBuild { get; set; }

    private Build(Project project, Guid id, BuildType buildType)
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
        return new Build(ProjectsService.Instance.Current, Guid.Parse(filename));
    }

    public static Build Load(Guid id)
    {
        return new Build(ProjectsService.Instance.Current, id);
    }

    public static Build New(BuildType buildType)
    {
        return new Build(ProjectsService.Instance.Current, Guid.NewGuid(), buildType);
    }

    public override string? Command => Builder.Command;

    public override Task<int> Compile(CancellationToken token = new()) => Builder.Compile(token: token);

    public override Task<ICompletedProcess>
        Run(string args = "", string? stdin = null, CancellationToken token = new()) =>
        Builder.Run(args, WorkingDirectory, stdin, Environment, token: token);

    public override Task<ICompletedProcess> RunConsole(string args = "", string? stdin = null,
        CancellationToken token = new()) =>
        Builder.RunConsole(args, WorkingDirectory, stdin, Environment, token: token);

    private ABuild? _getBuild(Guid id)
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
                code = await AppService.Instance
                    .RunBackgroundTask($"{Name} - компиляция", Compile, BackgroundTaskFlags.CanBeCancelled).Wait();
            }
            else if (proc.Command != null)
            {
                var lst = proc.Command.Split();
                code = (await AppService.Instance
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
                code = await AppService.Instance
                    .RunBackgroundTask($"{Name} - компиляция", Compile, BackgroundTaskFlags.CanBeCancelled).Wait();
            }
            else if (proc.Command != null)
            {
                var lst = proc.Command.Split();
                code = (await AppService.Instance
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

    public override async Task<int> RunPreProcConsole(CancellationToken token = new())
    {
        return await RunSubProcConsole(PreProc, token: token);
    }

    public override async Task<int> RunPostProcConsole(CancellationToken token = new())
    {
        return await RunSubProcConsole(PostProc, token: token);
    }

    public override async Task<int> RunPreProc(CancellationToken token = new())
    {
        return await RunSubProc(PreProc, token: token);
    }

    public override async Task<int> RunPostProc(CancellationToken token = new())
    {
        return await RunSubProc(PostProc, token: token);
    }

    public override async Task<int> ExecuteConsole(string args = "", string? stdin = null,
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

    public override async Task<int> Execute(string args = "", string? stdin = null, CancellationToken token = new())
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