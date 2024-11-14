using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Settings;

namespace TestGenerator.Settings;

public partial class SettingsWindow : Window
{
    private Dictionary<string, Control> _pages = [];
    private ObservableCollection<SettingsNode> Nodes { get; } = [];
    private string? _currentPage = null;

    public SettingsWindow()
    {
        InitializeComponent();
        TreeView.ItemsSource = Nodes;

        Add("Плагины", new PluginsView());
        Add("Проект", new SettingsPage("", [
            new StringField { Key = "name", FieldName = "Название проекта" },
        ], SettingsPage.SettingsPageType.ProjectData));

        ProjectsService.Instance.CurrentChanged += OnCurrentProjectChanged;
    }

    private void OnCurrentProjectChanged(Project project)
    {
        foreach (var page in _pages.Values.Select(c => c as SettingsPage))
        {
            switch (page?.Type)
            {
                case SettingsPage.SettingsPageType.ProjectSettings:
                    page.Section = project.Settings.GetSection(page.Key);
                    break;
                case SettingsPage.SettingsPageType.ProjectData:
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
                node = new SettingsNode { Name = name, FullName = folderName };
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

    public void Add(string pageName, Control control)
    {
        _pages[pageName] = control;
        CreateNode(pageName);
        PagesPanel.Children.Add(control);
        control.IsVisible = false;

        if (control is SettingsPage settingsPage)
        {
            switch (settingsPage.Type)
            {
                case SettingsPage.SettingsPageType.GlobalSettings:
                    settingsPage.Section = AppService.Instance.Settings.GetSection(settingsPage.Key);
                    break;
                case SettingsPage.SettingsPageType.ProjectSettings:
                    settingsPage.Section = ProjectsService.Instance.Current.Settings.GetSection(settingsPage.Key);
                    break;
                case SettingsPage.SettingsPageType.ProjectData:
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
                _pages[_currentPage].IsVisible = false;
            }

            _currentPage = node.FullName;
            _pages[_currentPage].IsVisible = true;
        }
    }
}