using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Core.Services;

public class AppService : AAppService
{
    private static AppService? _appService;

    public new static AppService Instance
    {
        get
        {
            if (_appService == null)
            {
                _instance = _appService = new AppService();
            }

            return _appService;
        }
    }

    public override Version? AppVersion => Version.TryParse("{AppVersion}", out var res) ? res : null;

    public override string AppDataPath { get; } = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko", Config.AppName);

    public override SettingsFile Settings { get; } = SettingsFile.Open(Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko",
        Config.AppName, "Settings.xml"));

    public override SettingsSection GetSettings(string key)
    {
        return Settings.GetSection(key);
    }

    public override SettingsSection GetSettings()
    {
        return GetSettings(PluginsService.Instance.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
    }

    public override string GetDataPath(string key)
    {
        var res = Path.Join(AppDataPath, "PluginData", key);
        Directory.CreateDirectory(res);
        return res;
    }

    public override string GetDataPath()
    {
        return GetDataPath(PluginsService.Instance.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
    }

    private readonly Dictionary<string, IEvent> _events = [];
    private readonly Dictionary<string, IRequestHandler> _requestHandlers = [];

    public delegate void ShowHandler(string key);

    public event ShowHandler? OnMainTabShow;
    public event ShowHandler? OnSideTabShow;

    public override void ShowMainTab(string key)
    {
        LogService.Logger.Debug($"Show main tab '{key}'");
        OnMainTabShow?.Invoke(key);
    }

    public override void ShowSideTab(string key)
    {
        LogService.Logger.Debug($"Show side tab '{key}'");
        OnSideTabShow?.Invoke(key);
    }

    public override Logger GetLogger(string name) => LogService.GetLogger(name);

    public override Logger GetLogger()
    {
        return GetLogger(PluginsService.Instance.GetPluginKeyByAssembly(Assembly.GetCallingAssembly()));
    }

    public override void Emit(string key, object? data = null)
    {
        if (_events.TryGetValue(key, out var ev))
        {
            ev.Emit(data);
        }
    }

    public override ISubscription Subscribe<T>(string key, Action<T> handler)
    {
        if (!_events.ContainsKey(key))
            _events[key] = new Event(key);
        return _events[key].Subscribe(handler);
    }

    public override ISubscription Subscribe(string key, Action handler)
    {
        if (!_events.ContainsKey(key))
            _events[key] = new Event(key);
        return _events[key].Subscribe(handler);
    }

    public override void AddRequestHandler<TI, TO>(string key, Func<TI, Task<TO>> handler)
    {
        var h = new RequestHandler(key);
        h.SetHandler(handler);
        _requestHandlers[key] = h;
    }

    public override void AddRequestHandler<TO>(string key, Func<Task<TO>> handler)
    {
        var h = new RequestHandler(key);
        h.SetHandler(handler);
        _requestHandlers[key] = h;
    }

    public override void AddRequestHandler<TI, TO>(string key, Func<TI, CancellationToken, Task<TO>> handler)
    {
        var h = new RequestHandler(key);
        h.SetHandler(handler);
        _requestHandlers[key] = h;
    }

    public override void AddRequestHandler<TO>(string key, Func<CancellationToken, Task<TO>> handler)
    {
        var h = new RequestHandler(key);
        h.SetHandler(handler);
        _requestHandlers[key] = h;
    }

    public override async Task<T> Request<T>(string key, object? data = null, CancellationToken token = new())
    {
        var res = await _requestHandlers[key].Call(data, token);
        if (res is T tRes)
            return tRes;
        var res2 = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(data));
        if (res2 != null)
            return res2;
        throw new InvalidCastException();
    }

    public override AProject CurrentProject => ProjectsService.Instance.Current;

    public override async Task<ICompletedProcess> RunProcess(RunProcessArgs.ProcessRunProvider where,
        RunProcessArgs args, CancellationToken token = new())
    {
        switch (where)
        {
            case RunProcessArgs.ProcessRunProvider.Background:
                return await RunBackgroundProcess(args, token: token);
            case RunProcessArgs.ProcessRunProvider.RunTab:
                return await Request<ICompletedProcess>("runProcessInTabRun", args, token);
            default:
                throw new Exception();
        }
    }

    public override async Task<ICompletedProcess> RunProcess(RunProcessArgs args, CancellationToken token = new())
    {
        return await RunProcess(RunProcessArgs.ProcessRunProvider.Background, args, token: token);
    }

    public override async Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs.ProcessRunProvider where,
        RunProcessArgs[] args, CancellationToken token = new())
    {
        var res = new List<ICompletedProcess>();
        switch (where)
        {
            case RunProcessArgs.ProcessRunProvider.Background:
                foreach (var arg in args)
                {
                    res.Add(await RunBackgroundProcess(arg, token: token));
                }

                break;
            case RunProcessArgs.ProcessRunProvider.RunTab:
                foreach (var arg in args)
                {
                    res.Add(await Request<ICompletedProcess>("runProcessInTabRun", arg, token));
                }

                break;
            default:
                throw new Exception();
        }

        return res;
    }

    public override async Task<ICollection<ICompletedProcess>> RunProcess(RunProcessArgs[] args, CancellationToken token = new())
    {
        return await RunProcess(RunProcessArgs.ProcessRunProvider.Background, args, token: token);
    }

    private async Task<ICompletedProcess> RunBackgroundProcess(RunProcessArgs args, CancellationToken token = new())
    {
        Process? proc;
        try
        {
            proc = Process.Start(new ProcessStartInfo(args.Filename, args.Args)
            {
                CreateNoWindow = true,
                RedirectStandardInput = args.Stdin != null,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = args.WorkingDirectory,
            });
        }
        catch (Exception e)
        {
            LogService.Logger.Error(e.Message);
            return new CompletedProcess { ExitCode = -1 };
        }

        if (proc == null)
        {
            LogService.Logger.Information($"Can not start '{args.Filename}'");
            return new CompletedProcess { ExitCode = -1 };
        }

        token.Register(proc.Kill);

        if (args.Stdin != null)
        {
            await proc.StandardInput.WriteAsync(args.Stdin);
            await proc.StandardInput.FlushAsync(token);
        }

        await proc.WaitForExitAsync(token);
        var filename = args.Filename.Contains(' ') ? "\"" + args.Filename + "\"" : args.Filename;
        LogService.Logger.Information($"Process '{filename} {args.Args}' (exit {proc.ExitCode})");
        return new CompletedProcess
        {
            ExitCode = proc.ExitCode,
            Stdout = await proc.StandardOutput.ReadToEndAsync(token),
            Stderr = await proc.StandardError.ReadToEndAsync(token),
            // Time = proc.TotalProcessorTime,
        };
    }

    public ObservableCollection<IBackgroundTask> BackgroundTasks { get; } = [];
    public ObservableCollection<IBackgroundTask> VisibleBackgroundTasks { get; } = [];

    public override IBackgroundTask RunBackgroundTask(string name, Func<IBackgroundTask, CancellationToken, Task<int>> func, BackgroundTaskFlags? flags = null)
    {
        return RunBackgroundTask(new BackgroundTask(name, func, flags));
    }
    
    public override IBackgroundTask RunBackgroundTask(string name, Func<CancellationToken, Task<int>> func, BackgroundTaskFlags? flags = null)
    {
        return RunBackgroundTask(new BackgroundTask(name, func, flags));
    }

    public override IBackgroundTask RunBackgroundTask(IBackgroundTask task)
    {
        task.Run();
        BackgroundTasks.Add(task);
        if ((task.Flags & BackgroundTaskFlags.Hidden) == 0)
            VisibleBackgroundTasks.Add(task);
        WaitBackgroundTask(task);
        return task;
    }

    private async void WaitBackgroundTask(IBackgroundTask task)
    {
        var code = await task.Wait();
        if (task is BackgroundTask backgroundTask)
            backgroundTask.ExitCode = code;
        if (task.IsCancelled)
            LogService.Logger.Information($"Task '{task.Name}' cancelled");
        else
            LogService.Logger.Information($"Task '{task.Name}' finished");
        BackgroundTasks.Remove(task);
        if ((task.Flags & BackgroundTaskFlags.Hidden) == 0)
            VisibleBackgroundTasks.Remove(task);
    }
}