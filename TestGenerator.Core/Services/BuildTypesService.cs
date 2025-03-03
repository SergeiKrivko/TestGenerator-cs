using AvaluxUI.Utils;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Services;

public class BuildTypesService
{
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

    public void Remove(BuildType buildType)
    {
        Types.Remove(buildType.Key);
    }

    public void Remove(string key)
    {
        Types.Remove(key);
    }

    private class CommandBuilder : BaseBuilder
    {
        public CommandBuilder(Guid id, IProject project, ISettingsSection settings) : base(id, project, settings)
        {
        }

        public override string Command => Settings.Get<string>("command", "");
    }

    public BuildTypesService(PluginsService pluginsService)
    {
        var bt = new BuildType
        {
            Name = "Команда",
            Key = "Command",
            Icon =
                "M3.05178e-05 4C3.05178e-05 4 3.05176e-05 7.53815e-06 4.00003 2.31943e-06H20C20 2.31943e-06 24 2.38419e-06 24 4V14C24 14 24 18 20 18H4.00003C4.00003 18 3.05178e-05 18 3.05178e-05 14V4ZM4.00003 1C4.00003 1 1 1 1.00003 4V14C1 17 4.00003 17 4.00003 17H20C20 17 23 17 23 14.5V4C23 1 20 1 20 1H4.00003ZM9 10.5C10.4 9 9 7.5 9 7.5L5 3L3.5 4.5L7.5 9L3.5 13.5L5 15L9 10.5ZM10 15H20V13H10V15Z",
            Builder = (id, project, settings) => new CommandBuilder(id, project, settings),
            SettingsFields = () =>
            [
                new StringField
                {
                    Key = "command",
                    FieldName = "Команда",
                    FontFamily = "Consolas",
                }
            ]
        };
        Types.Add("Command", bt);

        pluginsService.OnPluginLoaded += plugin =>
        {
            foreach (var buildType in plugin.BuildTypes)
            {
                Add(buildType);
            }
        };
        pluginsService.OnPluginUnloaded += plugin =>
        {
            foreach (var buildType in plugin.BuildTypes)
            {
                Remove(buildType);
            }
        };
    }
}