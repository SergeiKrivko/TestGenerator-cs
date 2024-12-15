using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public abstract class AAppService
{
    protected static AAppService? _instance;

    public static AAppService Instance
    {
        get
        {
            if (_instance == null)
                throw new Exception("App Service not initialized");
            return _instance;
        }
    }

    public abstract Version? AppVersion { get; }

    public abstract SettingsFile Settings { get; }

    public abstract string AppDataPath { get; }

    public abstract SettingsSection GetSettings(string key);
    public abstract SettingsSection GetSettings();

    public abstract string GetDataPath(string key);

    public abstract string GetDataPath();

    public abstract void ShowMainTab(string key);

    public abstract void ShowSideTab(string key);

    public abstract Logger GetLogger(string name);
    public abstract Logger GetLogger();

    public abstract void Emit(string key, object? data = null);

    public abstract ISubscription Subscribe<T>(string key, Action<T> handler);
    public abstract ISubscription Subscribe(string key, Action handler);

    public abstract Task<T> Request<T>(string key, object? data = null, CancellationToken token = new());
    public abstract void AddRequestHandler<TI, TO>(string key, Func<TI, Task<TO>> handler);
    public abstract void AddRequestHandler<TO>(string key, Func<Task<TO>> handler);
    public abstract void AddRequestHandler<TI, TO>(string key, Func<TI, CancellationToken, Task<TO>> handler);
    public abstract void AddRequestHandler<TO>(string key, Func<CancellationToken, Task<TO>> handler);

    public abstract AProject CurrentProject { get; }

    public abstract Task<ICompletedProcess> RunProcess(RunProcessArgs.ProcessRunProvider where, RunProcessArgs args, CancellationToken token = new());
    public abstract Task<ICompletedProcess> RunProcess(RunProcessArgs args, CancellationToken token = new());

    public abstract Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs.ProcessRunProvider where, RunProcessArgs[] args, CancellationToken token = new());
    public abstract Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs[] args, CancellationToken token = new());

    public abstract IBackgroundTask RunBackgroundTask(string name, Func<IBackgroundTask, CancellationToken, Task<int>> func, BackgroundTaskFlags? flags = null);
    public abstract IBackgroundTask RunBackgroundTask(string name, Func<CancellationToken, Task<int>> func, BackgroundTaskFlags? flags = null);
    public abstract IBackgroundTask RunBackgroundTask(IBackgroundTask task);
}