using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.UI;

public partial class BackgroundTasksBar : UserControl
{
    private IBackgroundTask? _currentTask;

    private readonly SettingsSection _developerSettings = AppService.Instance.GetSettings("Developer");
    private bool ShowAllTasks => _developerSettings.Get("showAllTasks", false);

    private ObservableCollection<IBackgroundTask> Tasks =>
        ShowAllTasks ? AppService.Instance.BackgroundTasks : AppService.Instance.VisibleBackgroundTasks;
    
    public BackgroundTasksBar()
    {
        InitializeComponent();
        ItemsControl.ItemsSource = Tasks;
        Tasks.CollectionChanged += BackgroundTasksOnCollectionChanged;
    }

    private void BackgroundTasksOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_currentTask != null)
                _currentTask.ProgressChanged -= CurrentTaskOnProgressChanged;
            _currentTask = Tasks.FirstOrDefault();
            IsVisible = _currentTask != null;
            if (_currentTask != null)
            {
                TaskNameBlock.Text = _currentTask.Name;
                CurrentTaskOnProgressChanged(_currentTask.Progress);
                _currentTask.ProgressChanged += CurrentTaskOnProgressChanged;
            }
            else
            {
                MainButton.Flyout?.Hide();
            }
        });
    }

    private void CurrentTaskOnProgressChanged(double? progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ProgressBar.IsIndeterminate = progress == null;
            if (progress != null)
                ProgressBar.Value = progress.Value;
        });
    }
}