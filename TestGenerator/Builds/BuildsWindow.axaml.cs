using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Core.Services;
using Core.Types;
using Shared.Settings;

namespace TestGenerator.Builds;

public partial class BuildsWindow : Window
{
    public ObservableCollection<Build> Builds { get; }
    
    public BuildsWindow()
    {
        InitializeComponent();
        Builds = BuildsService.Instance.Builds;
        BuildsList.ItemsSource = Builds;
        
        MainSettingsControl.Add(new StringField{Key = "name", FieldName = "Название"});
        MainSettingsControl.Add(new PathField{Key = "workingDirectory", FieldName = "Рабочая директория", Directory = true});
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        BuildsService.Instance.New("");
    }

    private void BuildsList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var build = BuildsList?.SelectedItems?[0] as Build;
        if (build == null)
            return;
        MainSettingsControl.Section = build.Settings;
        BuilderSettingsControl.Clear();
        foreach (var field in build.Builder.SettingsFields)
        {
            BuilderSettingsControl.Add(field);
        }
        BuilderSettingsControl.Section = build.Settings.GetSection("typeSettings");
        OtherSettingsControl.Section = build.Settings;
    }
}