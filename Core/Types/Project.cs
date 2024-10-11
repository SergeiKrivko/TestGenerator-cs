using Core.Services;
using Shared;
using Shared.Utils;

namespace Core.Types;

public class Project : AProject
{
    public override SettingsFile Settings { get; }
    public override SettingsFile Data { get; }
    
    public override Guid Id { get; }

    public override string Name
    {
        get => string.IsNullOrWhiteSpace(Data.Get("name")) ? System.IO.Path.GetFileName(Path) : Data.Get("name") ?? "";
        set => Settings.Set("name", value);
    }

    public override string Path { get; }

    public string DataPath => System.IO.Path.Join(Path, TestGeneratorDir);
    public override ProjectType Type { get; }

    private Project(string path)
    {
        Id = Guid.NewGuid();
        Path = path;
        Settings = SettingsFile.Open(System.IO.Path.Join(DataPath, "Settings.xml"));
        Data = SettingsFile.Open(System.IO.Path.Join(DataPath, "Data.xml"));
        Type = ProjectTypesService.Instance.Get(Data.Get("type") ?? "");
    }

    private Project()
    {
        Id = Guid.NewGuid();
        var path = Path = System.IO.Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko",
            Config.AppName, "LightEdit");
        Settings = SettingsFile.Open(System.IO.Path.Join(path, "Settings.xml"));
        Data = SettingsFile.Open(System.IO.Path.Join(path, "Data.xml"));
        if (Data.Get("name") != "LightEdit")
            Data.Set("name", "LightEdit");
        Type = ProjectTypesService.Default;
    }

    public static Project LightEditProject { get; } = new();

    public static Project Load(string path)
    {
        return new Project(path);
    }
}