using System.Text.Json;
using AvaluxUI.Utils;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public interface IProject
{
    public static string TestGeneratorDir { get; } = ".TestGenerator";

    public ISettingsSection Settings { get; }
    public ISettingsSection Data { get; }

    public ISettingsSection GetSettings();
    public ISettingsSection GetSettings(string key);

    public ISettingsSection GetData();
    public ISettingsSection GetData(string key);

    public Guid Id { get; }

    public string Name { get; set; }

    public string Path { get; }
    public string DataPath { get; }
    public ProjectType Type { get; }
}