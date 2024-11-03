using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public interface IField
{
    public object? Value { get; set; }
    public string? Key { get; set; }
    
    public delegate void ChangeHandler(object sender, object? value);

    public event ChangeHandler? ValueChanged;

    public void Load(SettingsSection section);
}