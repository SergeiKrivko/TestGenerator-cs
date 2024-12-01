using TestGenerator.Shared.Types;
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

    public delegate void Handler<T>(T obj);

    public delegate void Handler();

    public abstract ISubscription Subscribe<T>(string key, Handler<T> handler);
    public abstract ISubscription Subscribe(string key, Handler handler);

    public delegate Task<TO> RequestHandler<TI, TO>(TI data);

    public delegate Task<TO> RequestHandler<TO>();

    public abstract Task<T> Request<T>(string key, object? data = null);
    public abstract void AddRequestHandler<TI, TO>(string key, RequestHandler<TI, TO> handler);
    public abstract void AddRequestHandler<TO>(string key, RequestHandler<TO> handler);

    public abstract AProject CurrentProject { get; }

    public abstract Task<ICompletedProcess> RunProcess(RunProcessArgs.ProcessRunProvider where, RunProcessArgs args);
    public abstract Task<ICompletedProcess> RunProcess(RunProcessArgs args);

    public abstract Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs.ProcessRunProvider where, params RunProcessArgs[] args);
    public abstract Task<ICollection<ICompletedProcess>> RunProcess(params RunProcessArgs[] args);

    public delegate Task<int> BackgroundTaskProgressFunc(IBackgroundTask task);

    public delegate Task<int> BackgroundTaskFunc();

    public abstract IBackgroundTask RunBackgroundTask(string name, BackgroundTaskProgressFunc func);
    public abstract IBackgroundTask RunBackgroundTask(string name, BackgroundTaskFunc func);
    public abstract IBackgroundTask RunBackgroundTask(IBackgroundTask task);
}