﻿using Avalonia.Controls;
using AvaluxUI.Utils;

namespace TestGenerator.Shared.Settings;

public partial class SettingsControl : UserControl
{
    private ISettingsSection? _section;

    public ISettingsSection? Section
    {
        get => _section;
        set
        {
            _section = value;
            Load();
        }
    }

    private readonly List<IField> _fields = [];

    public SettingsControl()
    {
        InitializeComponent();
    }

    public SettingsControl(ICollection<IField> fields)
    {
        InitializeComponent();
        foreach (var field in fields)
        {
            Add(field);
        }
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