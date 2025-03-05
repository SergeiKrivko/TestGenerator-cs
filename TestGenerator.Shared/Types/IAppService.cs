using AvaluxUI.Utils;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public interface IAppService
{
    public Version? AppVersion { get; }

    public ISettingsSection Settings { get; }

    public string AppDataPath { get; }

    public ISettingsSection GetSettings(string key, bool encrypt = false);
    public ISettingsSection GetSettings(bool encrypt = false);

    public string GetDataPath(string key);

    public string GetDataPath();

    public void ShowMainTab(string key);

    public void ShowSideTab(string key);

    public Logger GetLogger(string name);
    public Logger GetLogger();

    public void Emit(string key, object? data = null);

    public ISubscription Subscribe<T>(string key, Action<T> handler);
    public ISubscription Subscribe(string key, Action handler);

    public Task<T> Request<T>(string key, object? data = null, CancellationToken token = new());
    public void AddRequestHandler<TI, TO>(string key, Func<TI, Task<TO>> handler);
    public void AddRequestHandler<TO>(string key, Func<Task<TO>> handler);
    public void AddRequestHandler<TI, TO>(string key, Func<TI, CancellationToken, Task<TO>> handler);
    public void AddRequestHandler<TO>(string key, Func<CancellationToken, Task<TO>> handler);

    public Task<ICompletedProcess> RunProcess(RunProcessArgs.ProcessRunProvider where, RunProcessArgs args, CancellationToken token = new());
    public Task<ICompletedProcess> RunProcess(RunProcessArgs args, CancellationToken token = new());

    public Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs.ProcessRunProvider where, RunProcessArgs[] args, CancellationToken token = new());
    public Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs[] args, CancellationToken token = new());

    public IBackgroundTask RunBackgroundTask(string name, Func<IBackgroundTask, CancellationToken, Task<int>> func, BackgroundTaskFlags? flags = null);
    public IBackgroundTask RunBackgroundTask(string name, Func<CancellationToken, Task<int>> func, BackgroundTaskFlags? flags = null);
    public IBackgroundTask RunBackgroundTask(IBackgroundTask task);
}