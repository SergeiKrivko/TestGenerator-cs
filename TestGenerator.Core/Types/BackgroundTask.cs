using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Types;

public class BackgroundTask : IBackgroundTask
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    public int? ExitCode { get; private set; }

    private double? _progress = 0;

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

    public event IBackgroundTask.StatusChangeHandler? StatusChanged;

    private Task<int>? _task;
    private AAppService.BackgroundTaskProgressFunc _func;

    public BackgroundTask(string name, AAppService.BackgroundTaskProgressFunc func)
    {
        Name = name;
        _func = func;
    }

    public BackgroundTask(string name, AAppService.BackgroundTaskFunc func)
    {
        Name = name;
        _progress = null;
        _func = task => func();
    }

    public Task Run()
    {
        LogService.Logger.Information($"Task {Name} started");
        return _task = Task.Run(() => _func(this));
    }

    public async Task<int> Wait()
    {
        if (_task == null)
            return ExitCode ?? 0;
        return await _task;
    }
}