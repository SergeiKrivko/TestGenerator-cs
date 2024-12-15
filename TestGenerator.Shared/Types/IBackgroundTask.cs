namespace TestGenerator.Shared.Types;

public interface IBackgroundTask
{
    public Guid Id { get; }
    public string Name { get; }
    public int? ExitCode { get; }
    
    public double? Progress { get; set; }
    public bool IsCancelled { get; }

    public bool IsIndeterminate => Progress == null;

    public delegate void ProgressChangeHandler(double? progress);

    public event ProgressChangeHandler? ProgressChanged;
    
    public string? Status { get; set; }

    public delegate void StatusChangeHandler(string? status);

    public event StatusChangeHandler? StatusChanged;

    public Task Run();

    public Task<int> Wait();

    public void Cancel();
    public Task CancelAsync();
    
    public BackgroundTaskFlags Flags { get; }
}

[Flags]
public enum BackgroundTaskFlags
{
    TerminateWithoutAsk = 1,
    ProjectTask = 2,
    Hidden = 4,
    UiThread = 8,
    CanBeCancelled = 16,
}