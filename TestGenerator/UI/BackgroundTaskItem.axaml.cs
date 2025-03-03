using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BackgroundTaskItem : UserControl
{
    private readonly AppService _appService = Injector.Inject<AppService>();

    public static readonly StyledProperty<IBackgroundTask?> BackgroundTaskProperty =
        AvaloniaProperty.Register<BackgroundTaskItem, IBackgroundTask?>(nameof(BackgroundTask));

    private readonly ISettingsSection _developerSettings;
    private bool AllowKillAllTasks => _developerSettings.Get<bool>("allowKillAllTasks");

    public IBackgroundTask? BackgroundTask
    {
        get => GetValue(BackgroundTaskProperty);
        set => SetValue(BackgroundTaskProperty, value);
    }

    public BackgroundTaskItem()
    {
        _developerSettings = _appService.GetSettings("Developer");
        InitializeComponent();
        PropertyChanged += (_, args) =>
        {
            if (args.Property == BackgroundTaskProperty && args.NewValue is IBackgroundTask task)
            {
                NameBlock.Text = task.Name;
                task.ProgressChanged += UpdateProgress;
                UpdateProgress(task.Progress);
                task.StatusChanged += UpdateStatus;
                UpdateStatus(task.Status);
                ButtonCancel.IsVisible = AllowKillAllTasks || (task.Flags & BackgroundTaskFlags.CanBeCancelled) > 0;
            }
        };
    }

    private void UpdateProgress(double? progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ProgressBar.IsIndeterminate = progress == null;
            if (progress != null)
                ProgressBar.Value = progress.Value;
        });
    }

    private void UpdateStatus(string? status)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusBlock.IsVisible = status != null;
            StatusBlock.Text = status;
        });
    }

    private void ButtonCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        BackgroundTask?.Cancel();
    }
}