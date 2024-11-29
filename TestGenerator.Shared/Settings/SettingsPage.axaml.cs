using System.Collections.Generic;
using Avalonia.Controls;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public partial class SettingsPage : UserControl
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
        InitializeComponent();
    }

    public SettingsPage(string key, ICollection<IField> fields, SettingsPageType type = SettingsPageType.GlobalSettings)
    {
        Key = key;
        Type = type;
        InitializeComponent();
        foreach (var field in fields)
        {
            Add(field);
        }
    }

    public void Clear() => SettingsControl.Clear();

    public void Add(IField field) => SettingsControl.Add(field);

    public SettingsSection? Section
    {
        get => SettingsControl.Section;
        set => SettingsControl.Section = value;
    }
}