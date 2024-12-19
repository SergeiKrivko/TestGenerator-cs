using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BuildRunner : UserControl
{
    public ObservableCollection<ABuild> Builds { get; }
    private IBackgroundTask? _task;

    public BuildRunner()
    {
        Builds = BuildsService.Instance.Builds;
        InitializeComponent();
        ComboBox.ItemsSource = Builds;
        ProjectsService.Instance.CurrentChanged += project =>
        {
            ComboBox.SelectedValue =
                BuildsService.Instance.Builds.FirstOrDefault(b =>
                    b.Id == project.Settings.Get<Guid>("selectedBuild"));
        };
    }

    private async void RunButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ComboBox.SelectedValue is ABuild build)
        {
            AppService.Instance.ShowSideTab("Run");
            _task = AppService.Instance.RunBackgroundTask($"Запуск {build.Name}",
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
        if (ComboBox.SelectedValue is ABuild build)
        {
            ProjectsService.Instance.Current.Settings.Set("selectedBuild", build.Id);
        }
    }
}