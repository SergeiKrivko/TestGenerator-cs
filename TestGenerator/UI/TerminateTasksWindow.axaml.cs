using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class TerminateTasksWindow : Window
{
    private readonly bool _isProjectTasks;
    private bool _success = false;

    private TerminateTasksWindow(bool isProjectTasks = false)
    {
        var lst =
            AppService.Instance.BackgroundTasks.Where(t =>
                (t.Flags & BackgroundTaskFlags.TerminateWithoutAsk) == 0 &&
                (!_isProjectTasks || (t.Flags & BackgroundTaskFlags.ProjectTask) > 0)).ToArray();
        if (lst.Length == 0)
        {
            _success = true;
            return;
        }
        _isProjectTasks = isProjectTasks;
        InitializeComponent();
        TasksList.ItemsSource = lst;
        AppService.Instance.BackgroundTasks.CollectionChanged += BackgroundTasksOnCollectionChanged;
    }

    private void BackgroundTasksOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var lst =
            AppService.Instance.BackgroundTasks.Where(t =>
                (t.Flags & BackgroundTaskFlags.TerminateWithoutAsk) == 0 &&
                (!_isProjectTasks || (t.Flags & BackgroundTaskFlags.ProjectTask) > 0)).ToArray();
        TasksList.ItemsSource = lst;
        if (lst.Length == 0)
        {
            _success = true;
            Close();
        }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TerminateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _success = true;
        Close();
    }

    private static async Task<bool> TerminateTasks(bool isProject)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow != null)
        {
            var window = new TerminateTasksWindow(isProject);
            if (!window._success)
                await window.ShowDialog(desktop.MainWindow);
            if (window._success)
            {
                foreach (var task in AppService.Instance.BackgroundTasks.Where(t =>
                             (t.Flags & BackgroundTaskFlags.TerminateWithoutAsk) == 0 &&
                             (!isProject || (t.Flags & BackgroundTaskFlags.ProjectTask) > 0)).ToArray())
                {
                    await task.CancelAsync();
                }
            }

            return window._success;
        }

        return true;
    }

    public static async Task<bool> TerminateAllTasks() => await TerminateTasks(false);
    public static async Task<bool> TerminateProjectTasks() => await TerminateTasks(true);
}