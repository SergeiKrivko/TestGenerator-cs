using System.Diagnostics;
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

    public override string AppDataPath { get; } = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko", Config.AppName);

    public override SettingsFile Settings { get; } = SettingsFile.Open(Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko",
        Config.AppName, "Settings.xml"));

    private Dictionary<string, IEvent> _events = [];
    private Dictionary<string, IRequestHandler> _requestHandlers = [];

    public delegate void ShowHandler(string key);

    public event ShowHandler? OnMainTabShow;
    public event ShowHandler? OnSideTabShow;

    public override void MainTabShow(string key)
    {
        LogService.Logger.Debug($"Show main tab '{key}'");
        OnMainTabShow?.Invoke(key);
    }

    public override void SideTabShow(string key)
    {
        LogService.Logger.Debug($"Show side tab '{key}'");
        OnSideTabShow?.Invoke(key);
    }

    public override Logger GetLogger(string name) => LogService.GetLogger(name);

    public delegate ITerminalController TerminalControllerFunc(string command, string? workingDirectory);

    public TerminalControllerFunc? TerminalController { get; set; }

    public override ITerminalController RunInConsole(string command, string? workingDirectory = null)
    {
        if (TerminalController == null)
            throw new Exception("Fail to run get TerminalController");
        return TerminalController(command, workingDirectory);
    }

    public override void Emit(string key, object? data = null)
    {
        if (_events.TryGetValue(key, out var ev))
        {
            ev.Emit(data);
        }
    }

    public override ISubscription Subscribe<T>(string key, Handler<T> handler)
    {
        if (!_events.ContainsKey(key))
            _events[key] = new Event(key);
        return _events[key].Subscribe(handler);
    }

    public override ISubscription Subscribe(string key, Handler handler)
    {
        if (!_events.ContainsKey(key))
            _events[key] = new Event(key);
        return _events[key].Subscribe(handler);
    }

    public override void AddRequestHandler<TI, TO>(string key, RequestHandler<TI, TO> handler)
    {
        var h = new RequestHandler(key);
        h.SetHandler(handler);
        _requestHandlers[key] = h;
    }

    public override void AddRequestHandler<TO>(string key, RequestHandler<TO> handler)
    {
        var h = new RequestHandler(key);
        h.SetHandler(handler);
        _requestHandlers[key] = h;
    }

    public override async Task<T> Request<T>(string key, object? data = null)
    {
        var res = await _requestHandlers[key].Call(data);
        if (data is T)
            return (T)res;
        var res2 = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(data));
        if (res2 != null)
            return res2;
        throw new InvalidCastException();
    }

    public override AProject CurrentProject => ProjectsService.Instance.Current;

    public override async Task<ICompletedProcess> RunProcess(string args)
    {
        var lst = args.Split();
        var proc = Process.Start(new ProcessStartInfo(lst[0], string.Join(' ', lst.Skip(1)))
        {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        });
        if (proc == null)
        {
            LogService.Logger.Information($"Can not start '{args}'");
            return new CompletedProcess { ExitCode = -1 };
        }

        await proc.WaitForExitAsync();
        LogService.Logger.Information($"Process '{args}' (exit {proc.ExitCode})");
        return new CompletedProcess
        {
            ExitCode = proc.ExitCode, 
            Stdout = await proc.StandardOutput.ReadToEndAsync(),
            Stderr = await proc.StandardError.ReadToEndAsync()
        };
    }
}