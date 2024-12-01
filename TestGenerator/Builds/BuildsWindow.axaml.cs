using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;

namespace TestGenerator.Builds;

public partial class BuildsWindow : Window
{
    public ObservableCollection<ABuild> Builds { get; }

    public BuildsWindow()
    {
        Builds = BuildsService.Instance.Builds;
        InitializeComponent();
        BuildsList.ItemsSource = Builds;

        var menu = new MenuFlyout();
        AddButton.Flyout = menu;
        foreach (var buildType in BuildTypesService.Instance.Types.Values)
        {
            var item = new BuildTypeItem(buildType);
            item.Selected += CreateBuild;
            menu.Items.Add(item);
        }

        MainSettingsControl.Add(new StringField { Key = "name", FieldName = "Название" });
        MainSettingsControl.Add(new PathField
            { Key = "workingDirectory", FieldName = "Рабочая директория", Directory = true });
        OtherSettingsControl.Add(new SubProcField{Key = "preProc", FieldName = " Перед выполнением"});
        OtherSettingsControl.Add(new SubProcField{Key = "postProc", FieldName = " После выполнения"});
    }

    private void CreateBuild(string type)
    {
        BuildsService.Instance.New(type);
    }

    private void BuildsList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (BuildsList.SelectedItems?.Count < 1)
            return;
        var build = BuildsList?.SelectedItems?[0] as Build;
        if (build == null)
            return;
        MainSettingsControl.Section = build.Settings;
        BuilderSettingsControl.Clear();
        foreach (var field in build.Type.SettingsFields())
        {
            BuilderSettingsControl.Add(field);
        }

        BuilderSettingsControl.Section = build.Settings.GetSection("typeSettings");
        OtherSettingsControl.Section = build.Settings;
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var build = BuildsList?.SelectedItems?[0] as Build;
        if (build == null)
            return;
        BuildsService.Instance.Remove(build);
    }
}