using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaEdit.Utils;
using TestGenerator.Core.Models;
using TestGenerator.Core.Services;

namespace TestGenerator.Settings;

public abstract partial class PluginsList : UserControl
{
    protected readonly PluginsHttpService HttpService = new();
    private RemotePlugin[] _plugins = [];
    public ObservableCollection<RemotePlugin> ObservablePlugins { get; } = [];

    private string? _selectedKey;
    private RemotePluginRelease? _latestRelease;
    private HashSet<string> _nowDownloading = [];

    public PluginsList()
    {
        InitializeComponent();
        ListBox.ItemsSource = ObservablePlugins;
        Load();
    }

    protected abstract Task<RemotePlugin[]> LoadAllPlugins();

    protected virtual RemotePluginRelease? LoadInstalledRelease(string key)
    {
        try
        {
            var plugin = PluginsService.Instance.Plugins[key];
            return new RemotePluginRelease
            {
                Id = default,
                PluginId = default,
                PublisherId = default,
                Name = plugin.Config.Name,
                Description = plugin.Config.Description,
                Runtime = default,
                Url = "",
                Version = plugin.Config.Version,
            };
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    protected abstract Task<RemotePluginRelease?> LoadLatestRelease(string key);

    public async void Load()
    {
        _plugins = await LoadAllPlugins();
        Search();
    }

    private void Search()
    {
        ObservablePlugins.Clear();
        if (string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            ObservablePlugins.AddRange(_plugins);
        }
        else
        {
            ObservablePlugins.AddRange(_plugins.Where(p => p.Key.Contains(SearchBox.Text)));
        }
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Search();
    }

    private async void Open(string? key)
    {
        _selectedKey = key;
        ReleasePanel.IsVisible = false;
        if (key == null)
        {
            PluginNameBox.Text = "";
            PluginDescriptionBox.Text = "";
            DownloadingBlock.IsVisible = false;
        }
        else
        {
            DownloadingBlock.IsVisible = _nowDownloading.Contains(key);
            _latestRelease = await LoadLatestRelease(key);
            var installedRelease = LoadInstalledRelease(key);

            PluginNameBox.Text = installedRelease?.Name ?? _latestRelease?.Name;
            VersionBlock.Text = (installedRelease?.Version ?? _latestRelease?.Version)?.ToString();
            PluginDescriptionBox.Text = installedRelease?.Description ?? _latestRelease?.Description;

            DownloadingBlock.IsVisible = _nowDownloading.Contains(key);
                InstallButton.IsVisible =
                !_nowDownloading.Contains(key) && installedRelease == null && _latestRelease != null;
            UpdateButton.IsVisible = !_nowDownloading.Contains(key) && installedRelease != null &&
                                     installedRelease.Version < _latestRelease?.Version;
            RemoveButton.IsVisible = !_nowDownloading.Contains(key) && installedRelease != null;
            
            ReleasePanel.IsVisible = true;
        }
    }

    private void ListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListBox.SelectedItems == null || ListBox.SelectedItems?.Count == 0)
            Open(null);
        else
        {
            Open((ListBox.SelectedItems?[0] as RemotePlugin)?.Key);
        }
    }

    private void AddDownloading(string key)
    {
        _nowDownloading.Add(key);
        if (_selectedKey == key)
        {
            DownloadingBlock.IsVisible = true;
            InstallButton.IsVisible = false;
            RemoveButton.IsVisible = false;
            UpdateButton.IsVisible = false;
        }
    }

    private void RemoveDownloading(string key)
    {
        _nowDownloading.Remove(key);
        if (_selectedKey == key)
        {
            DownloadingBlock.IsVisible = false;
            RemoveButton.IsVisible = true;
        }
    }

    private async void InstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_latestRelease != null && _selectedKey != null)
        {
            AddDownloading(_selectedKey);
            await PluginsService.Instance.Install(_latestRelease.Url);
            RemoveDownloading(_selectedKey);
        }
    }

    private async void RemoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_selectedKey != null)
        {
            await PluginsService.Instance.Remove(_selectedKey);
            Open(_selectedKey);
        }
    }

    private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_latestRelease != null && _selectedKey != null)
        {
            AddDownloading(_selectedKey);
            await PluginsService.Instance.Update(_selectedKey, _latestRelease.Url);
            RemoveDownloading(_selectedKey);
        }
    }
}