using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public partial class CodeTab : MainTab
{
    private readonly Dictionary<Guid, OpenedFile> _files = [];
    private readonly Dictionary<Guid, OpenedFileTab> _tabs = [];
    private Guid? _currentFileId = null;

    private static CodeTab? _instance = null;
    public static CodeTab? Instance => _instance;

    public OpenedFile? CurrentFile => _currentFileId == null ? null : _files[_currentFileId.Value];
    public List<IEditorProvider> Providers { get; } = [new CodeEditorProvider(), new SystemStdAppProvider()];

    public CodeTab()
    {
        _instance = this;
        InitializeComponent();
        AAppService.Instance.AddRequestHandler<string, string?>("openFile", Open);
        AAppService.Instance.AddRequestHandler<OpenFileWithModel, bool>("openFileWith", OpenWith);
        ProjectsService.Instance.CurrentChanged += OnProjectChanged;
    }

    private async void OnProjectChanged(Project project)
    {
        Clear();
        var current = project.Settings.Get<string>("currentFile");
        foreach (var file in project.Settings.Get<string[]>("openedFiles", []))
        {
            await Open(file);
        }

        var f = _files.Values.SingleOrDefault(f => f.Path == current);
        if (f != null)
            _tabs[f.Id].IsSelected = true;
    }

    private async Task<string?> Open(string path)
    {
        foreach (var provider in Providers.OrderByDescending(p => p.Priority))
        {
            if (provider.CanOpen(path))
            {
                OpenFileWithProvider(path, provider);
                return provider.Key;
            }
        }

        return null;
    }

    private async Task<bool> OpenWith(OpenFileWithModel model)
    {
        var provider = Providers.Find(p => p.Key == model.ProviderKey);
        if (provider == null)
            return false;
        OpenFileWithProvider(model.Path, provider);
        return true;
    }

    private void OpenFileWithProvider(string path, IEditorProvider provider)
    {
        var opened = provider.Open(path);
        if (opened == null)
            return;
        _files.Add(opened.Id, opened);
        EditorsPanel.Children.Add(opened.Widget);

        var tab = new OpenedFileTab(opened);
        _tabs.Add(opened.Id, tab);
        tab.Selected += TabOnSelected;
        tab.CloseRequested += CloseTab;
        TabsPanel.Children.Add(tab);
        tab.IsSelected = true;

        ProjectsService.Instance.Current.Settings.Set("openedFiles",
            _files.Values.Select(f => f.Path).ToArray());
    }

    private void CloseTab(OpenedFile file)
    {
        var tab = _tabs[file.Id];
        tab.Selected -= TabOnSelected;
        tab.CloseRequested -= CloseTab;
        TabsPanel.Children.Remove(tab);
        _tabs.Remove(file.Id);
        _files.Remove(file.Id);
        EditorsPanel.Children.Remove(file.Widget);
        if (tab.IsSelected && _tabs.Count > 0)
            _tabs.Values.First().IsSelected = true;
        ProjectsService.Instance.Current.Settings.Set("openedFiles",
            _files.Values.Select(f => f.Path).ToArray());
    }

    private void Clear()
    {
        foreach (var file in _files.Values.ToArray())
        {
            CloseTab(file);
        }
    }

    private void TabOnSelected(OpenedFile file)
    {
        foreach (var f in _files.Values.Where(f => f != file))
        {
            _tabs[f.Id].IsSelected = false;
            _files[f.Id].Widget.IsVisible = false;
        }

        _files[file.Id].Widget.IsVisible = true;
        ProjectsService.Instance.Current.Settings.Set("currentFile",
            file.Path);
    }
}