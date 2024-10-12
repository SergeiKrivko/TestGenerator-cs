using System.Collections.ObjectModel;
using System.Diagnostics;
using Shared.Settings;
using Shared.Utils;

namespace Shared;

public class BuildType
{
    public required string Name { get; init; }

    public required string Key { get; init; }

    public string Icon { get; init; } = "";

    public delegate Collection<IField> SettingsFunc();

    public SettingsFunc SettingsFields { get; init; } = () => [];

    public delegate void Handler(SettingsSection settings);

    public Handler? Compile { get; set; }

    public delegate string CommandBuilder(SettingsSection settings);

    public CommandBuilder? Command { get; init; }

    public Handler? Run { get; init; }

    public Handler? RunConsole { get; init; }

    public static BuildType Empty { get; } = new(){Name = "", Key = ""};
}