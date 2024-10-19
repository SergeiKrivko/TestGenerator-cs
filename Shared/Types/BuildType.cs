using System.Collections.ObjectModel;
using System.Diagnostics;
using Shared.Settings;
using Shared.Types;
using Shared.Utils;

namespace Shared;

public class BuildType
{
    public required string Name { get; init; }

    public required string Key { get; init; }

    public string Icon { get; init; } = "";

    public delegate Collection<IField> SettingsFunc();

    public SettingsFunc SettingsFields { get; init; } = () => [];

    public delegate BaseBuilder BuilderFunc(Guid id, AProject project, SettingsSection settings);

    public required BuilderFunc Builder { get; init; }

    private class EmptyBuilder : BaseBuilder
    {
        public EmptyBuilder(Guid id, AProject project, SettingsSection settings) : base(id, project, settings)
        {
        }
    }

    public static BuildType Empty { get; } = new()
        { Name = "", Key = "", Builder = (id, project, settings) => new EmptyBuilder(id, project, settings) };
}