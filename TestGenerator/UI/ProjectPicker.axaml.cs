using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;

namespace TestGenerator.UI;

public partial class ProjectPicker : UserControl
{
    public ProjectsService Service { get; } = ProjectsService.Instance;

    public ProjectPicker()
    {
        InitializeComponent();
        ProjectsService.Instance.CurrentChanged += _onCurrentChanged;
        ProjectsListBox.ItemsSource = ProjectsService.Instance.Projects;
        _onCurrentChanged(ProjectsService.Instance.Current);
    }

    private void _onCurrentChanged(Project project)
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
            await Service.SetCurrentProject(project);
        }
    }

    private async void LightEditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightEditButton.IsChecked = true;
        if (Service.Current != Project.LightEditProject)
        {
            await Service.SetCurrentProject(Project.LightEditProject);
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
            await ProjectsService.Instance.SetCurrentProject(ProjectsService.Instance.Load(files[0].Path.AbsolutePath));
        }
    }
}