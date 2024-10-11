using Avalonia.Controls;
using Shared.Utils;

namespace Shared.Settings;

public partial class StringField : UserControl, IField
{
    public object? Value
    {
        get => TextBox.Text;
        set => TextBox.Text = value?.ToString();
    }

    public string? Key { get; set; }

    public event IField.ChangeHandler? ValueChanged;

    public string? FieldName
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public StringField()
    {
        InitializeComponent();
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, TextBox.Text);
    }

    public void Load(SettingsSection section)
    {
        if (Key != null)
            Value = section.Get<string>(Key);
    }
}