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

    public override string Type => Settings.Get<string>("type") ?? "";
    public override BuildType Builder { get; }

    private readonly Project _project;
    public SettingsFile Settings { get; }

    private Build(Project project, Guid id)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));

        Builder = BuildTypesService.Instance.Get(Settings.Get<string>("type") ?? "");
    }

    private Build(Project project, Guid id, BuildType buildType)
    {
        Id = id;
        _project = project;
        Settings = SettingsFile.Open(Path.Join(_project.DataPath, "Builds", $"{id}.xml"));
        Builder = buildType;
        Settings.Set("type", buildType.Key);
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

    public override void Compile()
    {
        if (Builder.Compile != null)
        {
            Builder.Compile(Settings.GetSection("typeSettings"));
        }
    }

    public override void Run(string args = "")
    {
        if (Builder.Run != null)
        {
            Builder.Run(Settings.GetSection("typeSettings"));
        } 
        else if (Builder.Command != null)
        {
            var command = Builder.Command(Settings.GetSection("typeSettings")).Split();
            Process.Start(command[0], string.Join(' ', command.Skip(1)));
        }
    }

    public override void RunConsole(string args = "")
    {
        if (Builder.RunConsole != null)
        {
            Builder.RunConsole(Settings.GetSection("typeSettings"));
        } 
        else if (Builder.Command != null)
        {
            var command = Builder.Command(Settings.GetSection("typeSettings"));
            AppService.Instance.SideTabShow("Run");
            AppService.Instance.SideTabCommand("Run","run", command);
        }
    }
}