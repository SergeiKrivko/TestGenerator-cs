using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using AvaluxUI.Utils;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Settings;

namespace TestGenerator.Builds;

public partial class SubProcField : UserControl, IField
{
    public string? Key { get; set; }

    public object? Value { get; set; }
    public ObservableCollection<BuildSubprocess> Subprocesses { get; private set; } = [];

    public string? FieldName
    {
        get => NameLabel.Text;
        set => NameLabel.Text = value;
    }

    public event IField.ChangeHandler? ValueChanged;

    public SubProcField()
    {
        InitializeComponent();
        ListBox.ItemsSource = Subprocesses;
    }

    public void Load(ISettingsSection section)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));
        if (Key != null)
        {
            Value = Subprocesses = section.Get<ObservableCollection<BuildSubprocess>>(Key, []);
            ListBox.ItemsSource = Subprocesses;
        }
    }

    private async void AddCommandButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow != null)
        {
            var subprocess = new BuildSubprocess();
            var dialog = new CommandDialog(subprocess);
            await dialog.ShowDialog(desktop.MainWindow);
            if (subprocess.Command != null)
            {
                Subprocesses.Add(subprocess);
                ValueChanged?.Invoke(this, Subprocesses);
            }
        }
    }

    private async void AddBuildButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow != null)
        {
            var subprocess = new BuildSubprocess();
            var dialog = new SubBuildDialog(subprocess);
            await dialog.ShowDialog(desktop.MainWindow);
            if (subprocess.BuildId != null)
            {
                Subprocesses.Add(subprocess);
                ValueChanged?.Invoke(this, Subprocesses);
            }
        }
    }

    private void RemoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ListBox.SelectedItems?.Count < 1)
            return;
        var subprocess = ListBox.SelectedItems?[0] as BuildSubprocess;
        if (subprocess != null)
        {
            Subprocesses.Remove(subprocess);
            ValueChanged?.Invoke(this, Subprocesses);
        }
    }
}