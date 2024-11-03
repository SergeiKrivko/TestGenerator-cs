using Avalonia.Controls;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public partial class SettingsControl : UserControl
{
    private SettingsSection? _section;
    public SettingsSection? Section
    {
        get => _section;
        set
        {
            _section = value;
            Load();
        }
    }
    
    private List<IField> _fields = [];
    
    public SettingsControl()
    {
        InitializeComponent();
    }

    private void Load()
    {
        if (_section == null)
            return;
        foreach (var field in _fields)
        {
            if (field.Key != null)
            {
                field.Load(_section);
            }
        }
    }

    public void Add(IField field)
    {
        field.ValueChanged += FieldOnValueChanged;
        _fields.Add(field);
        if (field is Control)
            MainPanel.Children.Add((Control)field);
    }

    private void FieldOnValueChanged(object sender, object? value)
    {
        Section?.Set((sender as IField)?.Key, value);
    }

    public void Clear()
    {
        foreach (var field in _fields)
        {
            field.ValueChanged -= FieldOnValueChanged;
        }
        _fields.Clear();
        MainPanel.Children.Clear();
    }
}