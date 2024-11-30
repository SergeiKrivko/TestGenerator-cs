using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Settings;

public partial class SettingsWindow : Window
{
    private Dictionary<string, TestGenerator.Shared.Settings.SettingsNode> _pages = [];
    private ObservableCollection<SettingsNode> Nodes { get; } = [];
    private string? _currentPage;

    public SettingsWindow()
    {
        InitializeComponent();
        TreeView.ItemsSource = Nodes;

        Add(new Shared.Settings.SettingsNode("Плагины", new PluginsView()));
        Add(new SettingsPage("Проект", "", [
            new StringField { Key = "name", FieldName = "Название проекта:" },
            new SelectField<string>
            {
                Key = "type", FieldName = "Тип проекта:",
                Items = new ObservableCollection<SelectItem<string>>(ProjectTypesService.Instance.Types.Values.Select(
                    type =>
                        new SelectItem<string> { Name = type.Name, Icon = type.IconPath, Value = type.Key })),
            },
        ], SettingsPageType.ProjectData, () => ProjectsService.Instance.Current != Project.LightEditProject));

        foreach (var plugin in PluginsService.Instance.Plugins.Values)
        {
            foreach (var item in plugin.Plugin.SettingsControls.Where(c => c.IsVisible))
            {
                Add(item);
            }
        }

        Closed += OnClosed;

        ProjectsService.Instance.CurrentChanged += OnCurrentProjectChanged;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        PagesPanel.Children.Clear();
    }

    private void OnCurrentProjectChanged(Project project)
    {
        foreach (var page in _pages.Values.Select(c => c as SettingsPage))
        {
            switch (page?.Type)
            {
                case SettingsPageType.ProjectSettings:
                    page.Section = project.Settings.GetSection(page.Key);
                    break;
                case SettingsPageType.ProjectData:
                    page.Section = project.Data.GetSection(page.Key);
                    break;
            }
        }
    }

    private SettingsNode CreateNode(string folderName)
    {
        SettingsNode? node;
        if (folderName.Contains('/'))
        {
            var index = folderName.LastIndexOf('/');
            var parent = CreateNode(folderName.Substring(0, index));
            var name = folderName.Substring(index);
            node = parent.Children.FirstOrDefault(n => n.Name == name);
            if (node == null)
            {
                node = new SettingsNode { Name = name.Trim('/'), FullName = folderName };
                parent.Children.Add(node);
            }

            return node;
        }

        node = Nodes.FirstOrDefault(n => n.Name == folderName);
        if (node == null)
        {
            node = new SettingsNode { Name = folderName, FullName = folderName };
            Nodes.Add(node);
        }

        return node;
    }

    public void Add(TestGenerator.Shared.Settings.SettingsNode page)
    {
        _pages[page.Path] = page;
        CreateNode(page.Path);
        PagesPanel.Children.Add(page.Control);
        page.Control.IsVisible = false;

        if (page is SettingsPage settingsPage)
        {
            switch (settingsPage.Type)
            {
                case SettingsPageType.GlobalSettings:
                    settingsPage.Section = AppService.Instance.Settings.GetSection(settingsPage.Key);
                    break;
                case SettingsPageType.ProjectSettings:
                    settingsPage.Section = ProjectsService.Instance.Current.Settings.GetSection(settingsPage.Key);
                    break;
                case SettingsPageType.ProjectData:
                    settingsPage.Section = ProjectsService.Instance.Current.Data.GetSection(settingsPage.Key);
                    break;
            }
        }
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TreeView.SelectedItem is SettingsNode node)
        {
            if (_currentPage != null)
            {
                try
                {
                    _pages[_currentPage].Control.IsVisible = false;
                }
                catch (KeyNotFoundException)
                {
                }
            }

            _currentPage = node.FullName;
            try
            {
                _pages[_currentPage].Control.IsVisible = true;
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }
}