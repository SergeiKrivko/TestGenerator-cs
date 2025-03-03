using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class ProjectPicker : UserControl
{
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();

    public ProjectPicker()
    {
        InitializeComponent();
        _projectsService.CurrentChanged += _onCurrentChanged;
        ProjectsListBox.ItemsSource = _projectsService.Projects;
        _onCurrentChanged(_projectsService.Current);
    }

    private void _onCurrentChanged(IProject project)
    {
        CurrentPathIcon.Data = PathGeometry.Parse(project.Type.IconPath);
        CurrentNameBlock.Text = project.Name;
        LightEditButton.IsChecked = project == Project.LightEditProject;
        ProjectsListBox.SelectedItem = project == Project.LightEditProject ? null : project;
    }

    private async void ProjectsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        await Task.Delay(100);
        if (ProjectsListBox.SelectedItem is Project project)
        {
            await _projectsService.SetCurrentProject(project);
        }
    }

    private async void LightEditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightEditButton.IsChecked = true;
        if (_projectsService.Current != Project.LightEditProject)
        {
            await _projectsService.SetCurrentProject(Project.LightEditProject);
        }
    }

    private async void OpenProjectButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        var files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Открытие проекта",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await _projectsService.SetCurrentProject(_projectsService.Load(files[0].Path.AbsolutePath));
        }
    }

    private void CreateProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime lifetime &&
            lifetime.MainWindow != null)
        {
            var dialog = new CreateProjectWindow();
            dialog.ShowDialog(lifetime.MainWindow);
        }
    }
}