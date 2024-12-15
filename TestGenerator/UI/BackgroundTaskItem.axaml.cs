using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using Avalonia.Threading;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BackgroundTaskItem : UserControl
{
    public static readonly StyledProperty<IBackgroundTask?> BackgroundTaskProperty =
        AvaloniaProperty.Register<BackgroundTaskItem, IBackgroundTask?>(nameof(BackgroundTask));

    public IBackgroundTask? BackgroundTask
    {
        get => GetValue(BackgroundTaskProperty);
        set => SetValue(BackgroundTaskProperty, value);
    }

    public BackgroundTaskItem()
    {
        InitializeComponent();
        PropertyChanged += (sender, args) =>
        {
            if (args.Property == BackgroundTaskProperty && args.NewValue is IBackgroundTask task)
            {
                NameBlock.Text = task.Name;
                task.ProgressChanged += UpdateProgress;
                UpdateProgress(task.Progress);
                task.StatusChanged += UpdateStatus;
                UpdateStatus(task.Status);
                ButtonCancel.IsVisible = (task.Flags & BackgroundTaskFlags.CanBeCancelled) > 0;
            }
        };
    }

    private void UpdateProgress(double? progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ProgressBar.IsIndeterminate = progress == null;
            ProgressBar.Value = progress ?? 0;
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