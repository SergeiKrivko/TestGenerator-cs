﻿using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Core.Services;
using Core.Types;

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
            Title = "Открытие проекта",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            ProjectsService.Instance.Current = ProjectsService.Instance.Load(files[0].Path.AbsolutePath);
        }
    }
}