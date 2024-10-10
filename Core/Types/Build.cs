using Core.Services;
using Shared;
using Shared.Utils;

namespace Core.Types;

public class Build: ABuild
{
    public override Guid Id { get; }

    public override string Name
    {
        get => _settings.Get("name") ?? ""; 
        set => _settings.Set("name", value);
    }

    public override string WorkingDirectory
    {
        get => _settings.Get("workingDirectory") ?? _project.Path; 
        set => _settings.Set("workingDirectory", value);
    }

    public override string Type => _settings.Get("type") ?? "";
    public override BuildType Builder { get; }

    private readonly Project _project;
    private SettingsFile _settings;

    private Build(Project project, Guid id)
    {
        Id = id;
        _project = project;
        _settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));

        BuildType? builder;
        var buildType = BuildTypesService.Instance.Get(_settings.Get("type") ?? "");
        if (buildType?.IsSubclassOf(typeof(BuildType)) == true)
        {
            builder = Builder = Activator.CreateInstance(buildType) as BuildType ?? new EmptyBuild();
        }
        else
        {
            builder = Builder = new EmptyBuild();
        }

        builder.Settings = _settings.GetSection("TypeSettings");
    }

    private Build(Project project, Guid id, Type buildType)
    {
        Id = id;
        _project = project;
        _settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));
        
        BuildType? builder;
        if (buildType?.IsSubclassOf(typeof(BuildType)) == true)
        {
            builder = Builder = Activator.CreateInstance(buildType) as BuildType ?? new EmptyBuild();
        }
        else
        {
            builder = Builder = new EmptyBuild();
        }
        builder.Settings = _settings.GetSection("TypeSettings");
    }

    public static Build FromFile(string path)
    {
        var filename = Path.GetFileNameWithoutExtension(path);
        return new Build(ProjectsService.Instance.Current, Guid.Parse(filename));
    }

    public static Build New(BuildType type)
    {
        return new Build(ProjectsService.Instance.Current, Guid.NewGuid());
    }

    public override string Command => Builder.Command;

    public override void Compile() => Builder.Compile();

    public override void Run(string args) => Builder.Run(args);

    public override void RunConsole(string args) => Builder.RunConsole(args);
}