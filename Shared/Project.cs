using System.Text.Json;
using Shared.models;

namespace Shared;

public class Project
{
    public static string TestGeneratorDir { get; } = ".TestGenerator";
    public static string DataFile { get; } = "TestGeneratorData.json";

    public Guid Id { get; }
    private string? _name;

    public string Name
    {
        get => String.IsNullOrWhiteSpace(_name) ? System.IO.Path.GetFileName(Path) : _name;
        set => _name = value;
    }
    
    public string Path { get; }
    public ProjectType Type { get; }

    private Project(string name, string path, ProjectType type)
    {
        Id = Guid.NewGuid();
        Name = name;
        Path = path;
        Type = type;
    }

    public static Project LightEditProject { get; } = new("LightEdit", "", ProjectTypesService.Default);

    public static Project Load(string path)
    {
        var data = _loadData(path);
        return new Project(data.name ?? "", path, ProjectTypesService.Instance.Get(data.type ?? ""));
    }

    private static ProjectDataModel _loadData(string path)
    {
        var data = JsonSerializer.Deserialize<ProjectDataModel>(
            new StreamReader(System.IO.Path.Join(path, TestGeneratorDir, DataFile)).ReadToEnd());
        if (data == null)
            throw new Exception($"Can not read \"{DataFile}\" in {path}");
        return data;
    }
}