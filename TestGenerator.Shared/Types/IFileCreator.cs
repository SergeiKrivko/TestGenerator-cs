using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public interface IFileCreator
{
    public string Key { get; }
    public string Name { get; }
    public string? Icon { get; }
    public int Priority { get; }
    public bool Enabled => true;

    public SettingsControl? GetSettingsControl();

    public void Create(string root, SettingsSection options);
}