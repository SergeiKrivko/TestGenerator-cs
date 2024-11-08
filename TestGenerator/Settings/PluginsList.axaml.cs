using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;
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

    private async void Load()
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
        if (key == null)
        {
            PluginNameBox.Text = "";
            PluginDescriptionBox.Text = "";
        }
        else
        {
            _latestRelease = await LoadLatestRelease(key);
            var installedRelease = LoadInstalledRelease(key);
            
            PluginNameBox.Text = installedRelease?.Name ?? _latestRelease?.Name;
            PluginDescriptionBox.Text = installedRelease?.Description ?? _latestRelease?.Description;

            InstallButton.IsVisible = installedRelease == null && _latestRelease != null;
            UpdateButton.IsVisible = installedRelease != null && installedRelease.Version < _latestRelease?.Version;
            RemoveButton.IsVisible = installedRelease != null;
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

    private async void InstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_latestRelease != null)
        {
            await PluginsService.Instance.InstallPlugin(_latestRelease.Url);
        }
    }

    private void RemoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_selectedKey != null)
            PluginsService.Instance.RemovePlugin(_selectedKey);
    }

    private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_latestRelease != null && _selectedKey != null)
        {
            PluginsService.Instance.RemovePlugin(_selectedKey);
            await PluginsService.Instance.InstallPlugin(_latestRelease.Url);
        }
    }
}