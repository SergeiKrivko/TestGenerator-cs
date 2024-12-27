using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

public class BackgroundTask : IBackgroundTask
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    public int? ExitCode { get; internal set; }
    public bool IsCancelled { get; private set; }

    private double? _progress = null;

    public double? Progress
    {
        get => _progress;
        set
        {
            _progress = value == null ? null : double.Min(double.Max(value.Value, 0), 100);
            ProgressChanged?.Invoke(_progress);
        }
    }

    public event IBackgroundTask.ProgressChangeHandler? ProgressChanged;

    private string? _status;

    public string? Status
    {
        get => _status;
        set
        {
            _status = value;
            StatusChanged?.Invoke(_status);
        }
    }

    public BackgroundTaskFlags Flags { get; }

    public event IBackgroundTask.StatusChangeHandler? StatusChanged;

    private Task<int>? _task;
    private readonly Func<IBackgroundTask, CancellationToken, Task<int>> _func;
    private CancellationTokenSource? _cancellationToken;

    public BackgroundTask(string name, Func<IBackgroundTask, CancellationToken, Task<int>> func,
        BackgroundTaskFlags? flags = null)
    {
        Name = name;
        _func = func;
        if (flags != null)
            Flags = flags.Value;
    }
    public BackgroundTask(string name, Func<CancellationToken, Task<int>> func,
        BackgroundTaskFlags? flags = null)
    {
        Name = name;
        _func = (task, token) => func(token);
        if (flags != null)
            Flags = flags.Value;
    }

    public Task Run()
    {
        LogService.Logger.Information($"Task '{Name}' started");
        _cancellationToken = new CancellationTokenSource();
        if ((Flags & BackgroundTaskFlags.UiThread) == 0)
            return _task = Task.Run(() => _func(this, _cancellationToken.Token), _cancellationToken.Token);
        return _task = _func(this, _cancellationToken.Token);
    }

    public async Task<int> Wait()
    {
        if (_task == null)
            return ExitCode ?? 0;
        try
        {
            return await _task;
        }
        catch (TaskCanceledException)
        {
            IsCancelled = true;
            return 500;
        }
        catch (Exception e)
        {
            LogService.Logger.Error($"Error in background task '{Name}': {e.Message}");
            return -1;
        }
    }

    public void Cancel()
    {
        _cancellationToken?.Cancel();
    }

    public async Task CancelAsync()
    {
        if (_cancellationToken != null)
            await _cancellationToken.CancelAsync();
    }
}