using AvaluxUI.Utils;

namespace TestGenerator.Shared.Settings;

public class SettingsPage : SettingsNode
{
    public SettingsPage(string path,
        string key,
        ICollection<IField> fields,
        SettingsPageType type = SettingsPageType.GlobalSettings,
        IsVisibleFunction? visibleFunction = null
    ) : base(path, new SettingsPageControl(key, fields, type), visibleFunction)
    {
    }

    public void Clear() => (Control as SettingsPageControl)?.Clear();

    public void Add(IField field) => (Control as SettingsPageControl)?.Add(field);

    public SettingsPageType Type => (Control as SettingsPageControl)?.Type ?? SettingsPageType.GlobalSettings;
    public string Key => (Control as SettingsPageControl)?.Key ?? "";

    public ISettingsSection? Section
    {
        get => (Control as SettingsPageControl)?.Section;
        set
        {
            if (Control is SettingsPageControl control)
                control.Section = value;
        }
    }
}