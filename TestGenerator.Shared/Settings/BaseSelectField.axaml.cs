using Avalonia.Controls;
using AvaluxUI.Utils;

namespace TestGenerator.Shared.Settings;

public abstract partial class BaseSelectField : UserControl, IField
{
    public abstract object? Value { get; set; }

    public string? Key { get; set; }

    public event IField.ChangeHandler? ValueChanged;

    public string? FieldName
    {
        get => Label.Text;
        set => Label.Text = value;
    }
    
    public BaseSelectField()
    {
        InitializeComponent();
    }

    public abstract void Load(ISettingsSection section);

    private void Box_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
    }
}