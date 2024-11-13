﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public partial class CodeTab : MainTab
{
    private readonly Dictionary<Guid, OpenedFileModel> _files = [];
    private readonly Dictionary<Guid, OpenedFileTab> _tabs = [];
    private Guid? _currentFileId = null;

    private static CodeTab? _instance = null;
    public static CodeTab? Instance => _instance;

    public OpenedFile? CurrentFile => _currentFileId == null ? null : _files[_currentFileId.Value].File;
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
        ProjectsService.Instance.Current.Settings.Set("openedFiles",
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
            EditorsPanel.Children.Remove(file.File.Widget);
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
            if (f?.Id == null)
                continue;
            _tabs[f.Id].IsSelected = false;
            var opened = _files[f.Id].File;
            if (opened == null)
                continue;
            opened.Widget.IsVisible = false;
        }

        openedFile.Widget.IsVisible = true;
        ProjectsService.Instance.Current.Settings.Set("currentFile",
            file);
    }
}