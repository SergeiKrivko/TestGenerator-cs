using System.Collections.ObjectModel;
using AvaluxUI.Utils;
using TestGenerator.Shared.Settings;

namespace TestGenerator.Shared.Types;

public class BuildType
{
    public virtual required string Name { get; init; }

    public virtual required string Key { get; init; }

    public virtual string? Icon { get; init; } = "";

    public delegate Collection<IField> SettingsFunc();

    public virtual SettingsFunc SettingsFields { get; init; } = () => [];

    public delegate BaseBuilder BuilderFunc(Guid id, IProject project, ISettingsSection settings);

    public virtual required BuilderFunc Builder { get; init; }

    private class EmptyBuilder : BaseBuilder
    {
        public EmptyBuilder(Guid id, IProject project, ISettingsSection settings) : base(id, project, settings)
        {
        }
    }

    public static BuildType Empty { get; } = new()
        { Name = "", Key = "", Builder = (id, project, settings) => new EmptyBuilder(id, project, settings) };

    public virtual Task RefreshCreators() => Task.CompletedTask;
}