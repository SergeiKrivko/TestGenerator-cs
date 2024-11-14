using System.Collections.Generic;
using TestGenerator.Shared.Settings;

namespace TestGenerator.Settings;

public class SettingsPage : SettingsControl
{
    public enum SettingsPageType
    {
        GlobalSettings,
        ProjectSettings,
        ProjectData,
    }
    
    public virtual string Key { get; }
    public virtual SettingsPageType Type { get; }
    public virtual bool Enabled => true;

    public SettingsPage(string key, SettingsPageType type = SettingsPageType.GlobalSettings)
    {
        Key = key;
        Type = type;
    }

    public SettingsPage(string key, ICollection<IField> fields, SettingsPageType type = SettingsPageType.GlobalSettings) : base(fields)
    {
        Key = key;
        Type = type;
    }
}