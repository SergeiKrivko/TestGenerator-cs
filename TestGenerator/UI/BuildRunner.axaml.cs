using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BuildRunner : UserControl
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();
    private readonly BuildsService _buildsService = Injector.Inject<BuildsService>();
    
    public ObservableCollection<IBuild> Builds { get; }
    private IBackgroundTask? _task;

    public BuildRunner()
    {
        Builds = _buildsService.Builds;
        InitializeComponent();
        ComboBox.ItemsSource = Builds;
        _projectsService.CurrentChanged += project =>
        {
            ComboBox.SelectedValue =
                _buildsService.Builds.FirstOrDefault(b =>
                    b.Id == project.Settings.Get<Guid>("selectedBuild"));
        };
    }

    private async void RunButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ComboBox.SelectedValue is IBuild build)
        {
            _appService.ShowSideTab("Run");
            _task = _appService.RunBackgroundTask($"Запуск {build.Name}",
                token => build.ExecuteConsole(token: token),
                BackgroundTaskFlags.Hidden | BackgroundTaskFlags.ProjectTask | BackgroundTaskFlags.UiThread);
            ButtonCancel.IsVisible = true;
            ButtonRerun.IsVisible = true;
            ButtonRun.IsVisible = false;
            await _task.Wait();
            ButtonCancel.IsVisible = false;
            ButtonRerun.IsVisible = false;
            ButtonRun.IsVisible = true;
        }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _task?.Cancel();
    }

    private void ButtonRerun_OnClick(object? sender, RoutedEventArgs e)
    {
        _task?.Cancel();
        RunButton_OnClick(sender, e);
    }

    private void ComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ComboBox.SelectedValue is IBuild build)
        {
            _projectsService.Current.Settings.Set("selectedBuild", build.Id);
        }
    }
}