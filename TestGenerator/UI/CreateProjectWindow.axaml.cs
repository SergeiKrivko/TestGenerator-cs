using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class CreateProjectWindow : Window
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly ProjectTypesService _projectTypesService = Injector.Inject<ProjectTypesService>();
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();

    private readonly Dictionary<string, Control> _controls = [];

    public CreateProjectWindow()
    {
        InitializeComponent();
        Tree.ItemsSource = _projectTypesService.Types.Values.Select(type => new ProjectTypeNode
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
        MainView.IsVisible = false;
        ProgressView.IsVisible = true;

        var control = _controls[creator.Key];
        var path = creator.Creator.GetPath(control);
        Directory.CreateDirectory(path);
        var project = _projectsService.Create(path, creator.Type);
        await _projectsService.SetCurrentProject(project);

        var task = _appService.RunBackgroundTask("Создание проекта",
            async (task, token) =>
            {
                await creator.Creator.Initialize(project, control, task, token);
                return 0;
            });

        task.ProgressChanged += TaskOnProgressChanged;
        task.StatusChanged += TaskOnStatusChanged;
        TaskProgressBar.IsIndeterminate = task.Progress == null;
        TaskProgressBar.Value = task.Progress ?? 0;
        TaskStatusBar.Text = task.Status;

        await task.Wait();

        await _projectsService.ReloadProject();
        Close();
    }

    private void TaskOnStatusChanged(string? status)
    {
        Dispatcher.UIThread.Post(() => TaskStatusBar.Text = status);
    }

    private void TaskOnProgressChanged(double? progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            TaskProgressBar.IsIndeterminate = progress == null;
            TaskProgressBar.Value = progress ?? 0;
        });
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