using System.Reflection;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Core.Types;

public class Project : AProject
{
    public override SettingsFile Settings { get; }
    public override SettingsFile Data { get; }

    public override Guid Id { get; }

    public override string Name
    {
        get => string.IsNullOrWhiteSpace(Data.Get<string>("name"))
            ? System.IO.Path.GetFileName(Path)
            : Data.Get<string>("name") ?? "";
        set => Settings.Set("name", value);
    }

    public override string Path { get; }

    public string DataPath => System.IO.Path.Join(Path, TestGeneratorDir);
    public override ProjectType Type { get; }

    private Project(string path, ProjectType? projectType = null)
    {
        Id = Guid.NewGuid();
        Path = path;
        CreateDataDirectory();
        Settings = SettingsFile.Open(System.IO.Path.Join(DataPath, "Settings.xml"));
        var data = Data = SettingsFile.Open(System.IO.Path.Join(DataPath, "Data.xml"));
        var typeKey = data.Get<string>("type");
        Type = projectType ?? (typeKey == null ? DetectType() : ProjectTypesService.Instance.Get(typeKey));
    }

    private Project()
    {
        Id = Guid.NewGuid();
        var path = Path = System.IO.Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko",
            Config.AppName, "LightEdit");
        Settings = SettingsFile.Open(System.IO.Path.Join(path, "Settings.xml"));
        var data = Data = SettingsFile.Open(System.IO.Path.Join(path, "Data.xml"));
        if (data.Get<string>("name") != "LightEdit")
            data.Set("name", "LightEdit");
        Type = ProjectTypesService.Instance.Get(data.Get<string>("type", ""));
    }

    public static Project LightEditProject { get; } = new();

    public static Project Load(string path)
    {
        return new Project(path);
    }

    public static Project Create(string path, ProjectType type)
    {
        return new Project(path, type);
    }

    public override SettingsSection GetSettings(string key)
    {
        return Settings.GetSection(key);
    }

    public override SettingsSection GetSettings()
    {
        return GetSettings(PluginsService.Instance.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
    }

    public override SettingsSection GetData(string key)
    {
        return Data.GetSection(key);
    }

    public override SettingsSection GetData()
    {
        return GetData(PluginsService.Instance.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
    }

    private void CreateDataDirectory()
    {
        if (Directory.Exists(DataPath))
            return;
        Directory.CreateDirectory(DataPath);
        File.WriteAllText(System.IO.Path.Join(DataPath, ".gitignore"), "# Created automatically by TestGenerator\n" +
                                                                       "*\n" +
                                                                       "Settings.xml\n");
    }

    private ProjectType DetectType()
    {
        var detectors = new List<KeyValuePair<ProjectType, ProjectType.ProjectTypeDetector>>();
        foreach (var projectType in ProjectTypesService.Instance.Types.Values)
        {
            detectors.AddRange(projectType.Detectors.Select(d =>
                new KeyValuePair<ProjectType, ProjectType.ProjectTypeDetector>(projectType, d)));
        }

        foreach (var pair in detectors.OrderByDescending(p => p.Value.Priority))
        {
            LogService.Logger.Debug($"Trying detector with priority {pair.Value.Priority} for project type '{pair.Key.Key}'");
            if (pair.Value.Func(Path))
            {
                Data.Set("type", pair.Key.Key);
                return pair.Key;
            }
        }

        return ProjectTypesService.Default;
    }
}