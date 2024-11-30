using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public partial class DefaultField : UserControl, IField
{
    public string? Key { get; set; }

    public object? Value
    {
        get => CheckBox.IsChecked;
        set => CheckBox.IsChecked = value as bool?;
    }

    public event IField.ChangeHandler? ValueChanged;

    public bool Inversion { get; set; } = false;

    public string? FieldName
    {
        get => CheckBox.Content as string;
        set => CheckBox.Content = value;
    }

    public DefaultField()
    {
        InitializeComponent();
    }

    public DefaultField(ICollection<IField> fields)
    {
        InitializeComponent();
        foreach (var field in fields)
        {
            Add(field);
        }
    }

    private readonly List<IField> _fields = [];

    public void Add(IField field)
    {
        field.ValueChanged += FieldOnValueChanged;
        _fields.Add(field);
        if (field is Control control)
            ChildrenPanel.Children.Add(control);
    }

    public void Load(SettingsSection section)
    {
        if (Key != null)
            Value = section.Get<bool>(Key);
        ChildrenPanel.IsVisible = CheckBox.IsChecked == true;
        if (Inversion)
            ChildrenPanel.IsVisible = !ChildrenPanel.IsVisible;
        foreach (var field in _fields)
        {
            field.Load(section);
        }
    }

    private void FieldOnValueChanged(object sender, object? value)
    {
        ValueChanged?.Invoke(sender, value);
    }

    public void Clear()
    {
        foreach (var field in _fields)
        {
            field.ValueChanged -= FieldOnValueChanged;
        }

        _fields.Clear();
        ChildrenPanel.Children.Clear();
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
        ChildrenPanel.IsVisible = CheckBox.IsChecked == true;
        if (Inversion)
            ChildrenPanel.IsVisible = !ChildrenPanel.IsVisible;
    }
}