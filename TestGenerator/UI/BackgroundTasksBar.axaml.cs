using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BackgroundTasksBar : UserControl
{
    private readonly AppService _appService = Injector.Inject<AppService>();

    private IBackgroundTask? _currentTask;

    private readonly ISettingsSection _developerSettings;
    private bool ShowAllTasks => _developerSettings.Get("showAllTasks", false);

    private ObservableCollection<IBackgroundTask> Tasks =>
        ShowAllTasks ? _appService.BackgroundTasks : _appService.VisibleBackgroundTasks;

    public BackgroundTasksBar()
    {
        _developerSettings = _appService.GetSettings("Developer");
        
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