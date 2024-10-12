using System.Diagnostics;
using Core.Services;
using Shared;
using Shared.Utils;

namespace Core.Types;

public class Build: ABuild
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

    public override List<BuildSubprocess> PreProc  => Settings.Get<BuildSubprocess[]>("preProc", []).ToList();
    public override List<BuildSubprocess> PostProc  => Settings.Get<BuildSubprocess[]>("postProc", []).ToList();

    public override string Type => Settings.Get<string>("type") ?? "";
    public override BuildType Builder { get; }

    private readonly Project _project;
    public SettingsFile Settings { get; }

    private Build(Project project, Guid id)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));

        var builder = Builder = BuildTypesService.Instance.Get(Settings.Get<string>("type") ?? "");
    }

    public delegate Build? BuildGetter(Guid id);
    public BuildGetter? GetBuild { get; set; }

    private Build(Project project, Guid id, BuildType buildType)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));
        Builder = buildType;
        Settings.Set("type", buildType.Key);
        if (buildType.Init != null)
            buildType.Init(Settings);
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

    public override string? Command => Builder.Command == null ? null : Builder.Command(Settings.GetSection("typeSettings"));

    public override async Task<int> Compile()
    {
        if (Builder.Compile != null)
        {
            return await Builder.Compile(Settings.GetSection("typeSettings"));
        }

        return 0;
    }

    public override async Task<int> Run(string args = "")
    {
        if (Builder.Run != null)
        {
            return await Task.Run(() => Builder.Run(Settings.GetSection("typeSettings")));
        } 
        if (Builder.Command != null)
        {
            var command = Builder.Command(Settings.GetSection("typeSettings")).Split();
            var proc = Process.Start(command[0], string.Join(' ', command.Skip(1)));
            await proc.WaitForExitAsync();
            return proc.ExitCode;
        }

        throw new NullReferenceException("Both 'Command' and 'Run' can not be null");
    }

    public override async Task<int> RunConsole(string args = "")
    {
        if (Builder.RunConsole != null)
        {
            return await Builder.RunConsole(Settings.GetSection("typeSettings"));
        } 
        if (Builder.Command != null)
        {
            var command = Builder.Command(Settings.GetSection("typeSettings"));
            AppService.Instance.SideTabShow("Run");
            return await AppService.Instance.RunInConsole(command, WorkingDirectory).Run();
        }
        throw new NullReferenceException("Both 'Command' and 'RunConsole' can not be null");
    }

    private Build? _getBuild(Guid id)
    {
        if (GetBuild == null)
            throw new Exception("Can not get build");
        return GetBuild(id);
    }

    private async Task<int> RunSubProcConsole(List<BuildSubprocess> procs)
    {
        var code = 0;
        foreach (var proc in procs)
        {
            if (proc.Compile)
            {
                code = await Compile();
            }
            else if (proc.Command != null)
            {
                code = await AppService.Instance.RunInConsole(proc.Command, WorkingDirectory).Run();
            }
            else if (proc.BuildId != null)
            {
                var build = _getBuild(proc.BuildId.Value);
                if (build != null)
                    code = await build.ExecuteConsole();
            }
            if (code != 0)
                return code;
        }

        return code;
    }

    public override async Task<int> RunPreProcConsole()
    {
        return await RunSubProcConsole(PreProc);
    }

    public override async Task<int> RunPostProcConsole()
    {
        return await RunSubProcConsole(PostProc);
    }

    public override async Task<int> ExecuteConsole(string args = "")
    {
        var code = await RunPreProcConsole();
        if (code != 0)
            return code;
        code = await RunConsole(args);
        if (code != 0)
            return code;
        return await RunPostProcConsole();
    }
}