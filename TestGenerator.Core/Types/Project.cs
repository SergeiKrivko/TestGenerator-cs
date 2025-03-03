using System.Reflection;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

public class Project : IProject
{
    private readonly ProjectTypesService _projectTypesService = Injector.Inject<ProjectTypesService>();
    private readonly AppService _appService = Injector.Inject<AppService>();

    public ISettingsSection Settings { get; }
    public ISettingsSection Data { get; }

    public Guid Id { get; }

    public string Name
    {
        get => string.IsNullOrWhiteSpace(Data.Get<string>("name"))
            ? System.IO.Path.GetFileName(Path)
            : Data.Get<string>("name") ?? "";
        set => Settings.Set("name", value);
    }

    public string Path { get; }
    public const string TestGeneratorDir = ".TestGenerator";

    public string DataPath => System.IO.Path.Join(Path, TestGeneratorDir);
    public ProjectType Type { get; }

    private Project(string path, ProjectType? projectType = null)
    {
        Id = Guid.NewGuid();
        Path = path;
        CreateDataDirectory();
        Settings = SettingsFile.Open(System.IO.Path.Join(DataPath, "Settings.xml"));
        var data = Data = SettingsFile.Open(System.IO.Path.Join(DataPath, "Data.xml"));
        var typeKey = data.Get<string>("type");
        Type = projectType ?? (typeKey == null ? DetectType() : _projectTypesService.Get(typeKey));
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
        Type = _projectTypesService.Get(data.Get<string>("type", ""));
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

    public ISettingsSection GetSettings(string key)
    {
        return Settings.GetSection(key);
    }

    public ISettingsSection GetSettings()
    {
        return GetSettings(_appService.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
    }

    public ISettingsSection GetData(string key)
    {
        return Data.GetSection(key);
    }

    public ISettingsSection GetData()
    {
        return GetData(_appService.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
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
        foreach (var projectType in _projectTypesService.Types.Values)
        {
            detectors.AddRange(projectType.Detectors.Select(d =>
                new KeyValuePair<ProjectType, ProjectType.ProjectTypeDetector>(projectType, d)));
        }

        foreach (var pair in detectors.OrderByDescending(p => p.Value.Priority))
        {
            LogService.Logger.Debug(
                $"Trying detector with priority {pair.Value.Priority} for project type '{pair.Key.Key}'");
            if (pair.Value.Func(Path))
            {
                Data.Set("type", pair.Key.Key);
                return pair.Key;
            }
        }

        return ProjectTypesService.Default;
    }
}