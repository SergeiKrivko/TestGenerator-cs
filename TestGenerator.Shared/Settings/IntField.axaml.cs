using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public partial class IntField : UserControl, IField
{
    public object? Value
    {
        get => (int?)SpinBox.Value;
        set => SpinBox.Value = value as int?;
    }

    public string? Key { get; set; }

    public event IField.ChangeHandler? ValueChanged;

    public string? FieldName
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public IntField()
    {
        InitializeComponent();
    }

    public void Load(SettingsSection section)
    {
        if (Key != null)
            Value = section.Get<int>(Key);
    }

    private void SpinBox_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
    }
}