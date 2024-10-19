using Core.Services;
using Shared;
using Shared.Types;
using Shared.Utils;

namespace Core.Types;

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

        Console.WriteLine(BuildTypesService.Instance.Types.Count);
        var type = Type = BuildTypesService.Instance.Get(Settings.Get<string>("type") ?? "");
        Builder = type.Builder(id, _project, Settings.GetSection("typeSettings"));
    }

    public delegate Build? BuildGetter(Guid id);

    public BuildGetter? GetBuild { get; set; }

    private Build(Project project, Guid id, BuildType buildType)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));
        Type = buildType;
        Settings.Set("type", buildType.Key);
        var builder = Builder = buildType.Builder(id, _project, Settings.GetSection("typeSettings"));
        Console.WriteLine($"{builder.GetType().GetMethod("Compile")?.Module} " +
                          $"{typeof(BaseBuilder).GetMethod("Compile")?.Module}");
        if (builder.GetType().GetMethod("Compile") != typeof(BaseBuilder).GetMethod("Compile"))
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

    public override Task<int> Compile() => Builder.Compile();

    public override Task<int> Run(string args = "") => Builder.Run(args);

    public override Task<int> RunConsole(string args = "") => Builder.RunConsole(args);

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
                code = await AppService.Instance.RunInConsole(proc.Command, WorkingDirectory).RunAsync();
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