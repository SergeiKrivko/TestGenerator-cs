using Core.Types;
using Shared;
using Shared.Settings;

namespace Core.Services;

public class BuildTypesService
{
    private static BuildTypesService? _instance;

    public static BuildTypesService Instance
    {
        get
        {
            _instance ??= new BuildTypesService();
            return _instance;
        }
    }

    public Dictionary<string, BuildType> Types { get; } = [];

    public BuildType Get(string key)
    {
        return Types.GetValueOrDefault(key, BuildType.Empty);
    }

    public void Add(string key, BuildType buildType)
    {
        Types.Add(key, buildType);
    }

    public void Add(BuildType buildType)
    {
        Types.Add(buildType.Key, buildType);
    }

    private BuildTypesService()
    {
        Types.Add("Command", new BuildType
        {
            Name = "Команда",
            Key = "Command",
            Command = settings => settings.Get<string>("command", ""),
            SettingsFields = [new StringField
            {
                Key = "command",
                FieldName = "Команда",
            }]
        });
    }
}