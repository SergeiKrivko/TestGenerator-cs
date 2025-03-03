using Avalonia.Controls;
using AvaluxUI.Utils;

namespace TestGenerator.Shared.Settings;

internal partial class SettingsPageControl : UserControl
{
    public virtual string Key { get; }
    public virtual SettingsPageType Type { get; }

    public SettingsPageControl(string key, SettingsPageType type = SettingsPageType.GlobalSettings)
    {
        Key = key;
        Type = type;
        InitializeComponent();
    }

    public SettingsPageControl(string key, ICollection<IField> fields, SettingsPageType type = SettingsPageType.GlobalSettings)
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

    public ISettingsSection? Section
    {
        get => SettingsControl.Section;
        set => SettingsControl.Section = value;
    }
}