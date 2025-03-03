using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Settings;

public partial class SettingsWindow : Window
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();
    private readonly ProjectTypesService _projectTypesService = Injector.Inject<ProjectTypesService>();
    private readonly PluginsService _pluginsService = Injector.Inject<PluginsService>();

    private readonly Dictionary<string, TestGenerator.Shared.Settings.SettingsNode> _pages = [];
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
                Items = new ObservableCollection<SelectItem<string>>(_projectTypesService.Types.Values.Select(
                    type =>
                        new SelectItem<string> { Name = type.Name, Icon = type.IconPath, Value = type.Key })),
            },
        ], SettingsPageType.ProjectData, () => _projectsService.Current != Project.LightEditProject));

        foreach (var plugin in _pluginsService.Plugins.Values)
        {
            foreach (var item in plugin.Plugin.SettingsControls.Where(c => c.IsVisible))
            {
                Add(item);
            }
        }

        Add(new Shared.Settings.SettingsNode("Для разработчиков", new DeveloperPage()));

        Closed += OnClosed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        PagesPanel.Children.Clear();
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
                if (_pages[_currentPage] is SettingsPage settingsPage)
                {
                    switch (settingsPage.Type)
                    {
                        case SettingsPageType.GlobalSettings:
                            settingsPage.Section = _appService.Settings.GetSection(settingsPage.Key);
                            break;
                        case SettingsPageType.ProjectSettings:
                            settingsPage.Section =
                                _projectsService.Current.Settings.GetSection(settingsPage.Key);
                            break;
                        case SettingsPageType.ProjectData:
                            settingsPage.Section = _projectsService.Current.Data.GetSection(settingsPage.Key);
                            break;
                    }
                }
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }
}