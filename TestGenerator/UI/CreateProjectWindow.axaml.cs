using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class CreateProjectWindow : Window
{
    private Dictionary<string, Control> _controls = [];

    public CreateProjectWindow()
    {
        InitializeComponent();
        Tree.ItemsSource = ProjectTypesService.Instance.Types.Values.Select(type => new ProjectTypeNode
        {
            Type = type,
            Creators = type.Creators.Select(creator => new ProjectCreatorNode { Creator = creator, Type = type })
                .ToArray()
        }).Where(t => t.Creators.Count > 0);
    }

    private void Tree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Tree.SelectedItem is ProjectCreatorNode creator)
        {
            ButtonCreate.IsEnabled = true;
            if (!_controls.ContainsKey(creator.Key))
            {
                _controls[creator.Key] = creator.Creator.GetControl();
                CreatorsPanel.Children.Add(_controls[creator.Key]);
            }

            foreach (var pair in _controls)
            {
                pair.Value.IsVisible = creator.Key == pair.Key;
            }
        }
        else
        {
            ButtonCreate.IsEnabled = false;
            foreach (var pair in _controls)
            {
                pair.Value.IsVisible = false;
            }
        }
    }

    private async void ButtonCreate_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Tree.SelectedItem is not ProjectCreatorNode creator)
            return;

        var control = _controls[creator.Key];
        var path = creator.Creator.GetPath(control);
        Directory.CreateDirectory(path);
        var project = ProjectsService.Instance.Create(path, creator.Type);
        await ProjectsService.Instance.SetCurrentProject(project);

        await AppService.Instance.RunBackgroundTask("Создание проекта",
            async token =>
            {
                await creator.Creator.Initialize(project, control);
                return 0;
            }).Wait();

        await ProjectsService.Instance.ReloadProject();
        Close();
    }

    private void ButtonCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class ProjectTypeNode
    {
        public required ProjectType Type { get; init; }
        public required ICollection<ProjectCreatorNode> Creators { get; init; }

        public string Icon => Type.IconPath;
        public string Name => Type.Name;
    }

    public class ProjectCreatorNode
    {
        public required ProjectType Type { get; init; }
        public required IProjectCreator Creator { get; init; }

        public string? Icon => Creator.Icon;
        public string Name => Creator.Name;
        public string Key => $"{Type.Key}->{Creator.Icon}";
    }