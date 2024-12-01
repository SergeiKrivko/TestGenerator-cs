using Avalonia;
using Avalonia.Controls;
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
        BackgroundTaskProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<IBackgroundTask?>>(value =>
            {
                if (value.NewValue.Value != null)
                {
                    NameBlock.Text = value.NewValue.Value.Name;
                    value.NewValue.Value.ProgressChanged += UpdateProgress;
                    UpdateProgress(value.NewValue.Value.Progress);
                    value.NewValue.Value.StatusChanged += UpdateStatus;
                    UpdateStatus(value.NewValue.Value.Status);
                }
            }));
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
}