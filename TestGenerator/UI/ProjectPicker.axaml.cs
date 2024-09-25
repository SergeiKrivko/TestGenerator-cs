using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Backend;
using Shared;

namespace TestGenerator.UI;

public partial class ProjectPicker : UserControl
{
    public ProjectsService Service { get; } = ProjectsService.Instance;

    public ProjectPicker()
    {
        Console.WriteLine(ProjectsService.Instance.Current.Name);
        InitializeComponent();
        ProjectsService.Instance.CurrentChanged += _onCurrentChanged;
        ProjectsService.Instance.Load("C:\\Users\\sergi\\PycharmProjects\\GPT-chat");
        ProjectsService.Instance.Load("C:\\Users\\sergi\\PycharmProjects\\TestGen\\TestGenerator");
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

    private void ProjectsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ProjectsListBox.SelectedItem is Project)
        {
            var project = (Project)ProjectsListBox.SelectedItem;
            if (project != Service.Current)
            {
                Service.Current = project;
            }
        }
    }

    private void LightEditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightEditButton.IsChecked = true;
        if (Service.Current != Project.LightEditProject)
        {
            Service.Current = Project.LightEditProject;
        }
    }
    
    private async void OpenProjectButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        var files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Project",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            ProjectsService.Instance.Load(files[0].Path.AbsolutePath);
        }
    }
}