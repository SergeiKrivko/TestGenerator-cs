using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using AvaluxUI.Utils;
using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Shared.Settings;

public partial class EnvironmentField : UserControl, IField
{
    private ObservableCollection<EnvironmentVariable> Variables { get; } = [];

    public object? Value
    {
        get => new EnvironmentModel { Variables = Variables.ToArray(), InheritGlobal = InheritBox.IsChecked ?? false };
        set
        {
            Variables.Clear();
            if (value is EnvironmentModel environment)
            {
                foreach (var variable in environment.Variables)
                {
                    Variables.Add(variable);
                }

                InheritBox.IsChecked = environment.InheritGlobal;
            }
            else
            {
                InheritBox.IsChecked = true;
            }
        }
    }

    public string? Key { get; set; }

    public event IField.ChangeHandler? ValueChanged;

    public string? FieldName
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public EnvironmentField()
    {
        InitializeComponent();
        ItemsControl.ItemsSource = Variables;
        Variables.CollectionChanged += (s, e) => ValueChanged?.Invoke(this, Value);
    }

    public void Load(ISettingsSection section)
    {
        if (Key != null)
            Value = section.Get<EnvironmentModel>(Key);
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Variables.Add(new EnvironmentVariable());
    }

    private void InheritBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not StyledElement control)
            return;
        while (control != null && control is not ContentPresenter)
        {
            control = control.Parent;
        }

        if (control?.DataContext is EnvironmentVariable environmentVariable)
        {
            Variables.Remove(environmentVariable);
        }
    }
}