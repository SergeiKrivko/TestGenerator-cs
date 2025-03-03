using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public partial class CodeTab : MainTab
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();

    public override string TabKey => "Code";
    public override string TabName => "Код";
    public override int TabPriority => 100;

    private readonly Dictionary<Guid, OpenedFileModel> _files = [];
    private readonly Dictionary<Guid, OpenedFileTab> _tabs = [];
    private readonly Guid? _currentFileId = null;
    public static CodeTab? Instance { get; private set; }

    public OpenedFile? CurrentFile => _currentFileId == null ? null : _files[_currentFileId.Value].File;
    public List<IEditorProvider> Providers { get; } = [new CodeEditorProvider(), new SystemStdAppProvider()];

    public CodeTab()
    {
        Instance = this;
        InitializeComponent();
        _appService.AddRequestHandler<string, string?>("openFile", Open);
        _appService.AddRequestHandler<OpenFileWithModel, bool>("openFileWith", OpenWith);
        _projectsService.CurrentChanged += OnProjectChanged;
    }

    private async void OnProjectChanged(IProject project)
    {
        Clear();
        var current = project.Settings.Get<OpenedFileModel>("currentFile");
        foreach (var file in project.Settings.Get<OpenedFileModel[]>("openedFiles", []))
        {
            var provider = Providers.Find(p => p.Key == file.Provider);
            if (provider != null)
                OpenFileWithProvider(file.Path, provider);
            else
                await Open(file.Path);
        }

        var f = _files.Values.FirstOrDefault(f => f.Path == current?.Path && f.Provider == current.Provider);
        if (f != null)
            _tabs[f.Id].IsSelected = true;
    }

    private Task<string?> Open(string path)
    {
        try
        {
            foreach (var provider in Providers.OrderByDescending(p => p.Priority))
            {
                if (provider.CanOpen(path))
                {
                    OpenFileWithProvider(path, provider);
                    return Task.FromResult<string?>(provider.Key);
                }
            }
        }
        catch (Exception e)
        {
            LogService.Logger.Error($"Error while opening '{path}': {e.Message}");
        }

        return Task.FromResult<string?>(null);
    }

    private Task<bool> OpenWith(OpenFileWithModel model)
    {
        var provider = Providers.Find(p => p.Key == model.ProviderKey);
        if (provider == null)
            return Task.FromResult(false);
        try
        {
            OpenFileWithProvider(model.Path, provider);
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            LogService.Logger.Error($"Error while opening '{model.Path}' with {model.ProviderKey}: {e.Message}");
            return Task.FromResult(false);
        }
    }

    private bool ShowFileIfOpened(string path, IEditorProvider provider)
    {
        var openedFile = _files.Values.FirstOrDefault(f => f.Path == path && f.Provider == provider.Key);
        if (openedFile == null)
            return false;
        _tabs[openedFile.Id].IsSelected = true;
        return true;
    }

    private void OpenFileWithProvider(string path, IEditorProvider provider)
    {
        if (ShowFileIfOpened(path, provider))
            return;
        var opened = provider.Open(path);
        if (opened == null)
            return;
        var model = new OpenedFileModel { File = opened, Path = path, Provider = provider.Key };
        _files.Add(model.Id, model);
        EditorsPanel.Children.Add(opened.Widget);

        var tab = new OpenedFileTab(model);
        _tabs.Add(model.Id, tab);
        tab.Selected += TabOnSelected;
        tab.CloseRequested += CloseTab;
        TabsPanel.Children.Add(tab);
        tab.IsSelected = true;

        StoreOpenedFiles();
    }

    private void StoreOpenedFiles()
    {
        _projectsService.Current.Settings.Set("openedFiles",
            _files.Values.ToArray());
    }

    private void CloseTab(OpenedFileModel? file)
    {
        if (file == null)
            return;
        var tab = _tabs[file.Id];
        tab.Selected -= TabOnSelected;
        tab.CloseRequested -= CloseTab;
        TabsPanel.Children.Remove(tab);
        _tabs.Remove(file.Id);
        _files.Remove(file.Id);
        if (file.File?.Widget != null)
        {
            try
            {
                file.File.Widget.IsVisible = false;
                EditorsPanel.Children.Remove(file.File.Widget);
            }
            catch (Exception e)
            {
                LogService.Logger.Error(e.Message);
            }
        }

        if (tab.IsSelected && _tabs.Count > 0)
            _tabs.Values.First().IsSelected = true;
        StoreOpenedFiles();
    }

    private void Clear()
    {
        foreach (var file in _files.Values.ToArray())
        {
            CloseTab(file);
        }
    }

    private void TabOnSelected(OpenedFileModel file)
    {
        var openedFile = _files[file.Id].File;
        if (openedFile == null)
            return;
        foreach (var f in _files.Values.Where(f => f != file))
        {
            _tabs[f.Id].IsSelected = false;
            var opened = _files[f.Id].File;
            if (opened == null)
                continue;
            opened.Widget.IsVisible = false;
        }

        openedFile.Widget.IsVisible = true;
        _projectsService.Current.Settings.Set("currentFile", file);
    }
}