using System.Collections.ObjectModel;
using Avalonia.Controls;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Utils;

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

    public abstract void Load(SettingsSection section);

    private void Box_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
    }
}