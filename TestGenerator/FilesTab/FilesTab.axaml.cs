using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.MainTabs.Code;
using TestGenerator.Shared.Types;

namespace TestGenerator.FilesTab;

public partial class FilesTab : SideTab
{
    private ObservableCollection<Node> Nodes { get; }

    public override string TabName => "Файлы";
    public override string TabKey => "Files";

    public override string TabIcon =>
        "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z";

    public List<IFileCreator> BuiltinFileCreators { get; } = [new FileCreator(), new DirectoryCreator()];
    public List<IFileCreator> FileCreators { get; } = [];
    public List<IFileAction> FileActions { get; } = [];

    public FilesTab()
    {
        Node.RegexIcons[new Regex(@"[/\\]\.TestGenerator$")] = "M1 6C1 3 4 3 4 3H7.5C8 3 8.5 3.375 9 3.75C9.5 4.125 10 4.5 10.5 4.5H20.5C23.5 4.5 23.5 7.5 23.5 7.5V18C23.5 21.1354 20.5 21 20.5 21H4C4 21 1 21 1 18V6ZM4 4.5C4 4.5 2.5 4.5 2.5 6V18C2.5 18 2.5 19.5 4 19.5H20.5C20.5 19.5 22 19.5 22 18V7.5C22 7.5 22 6 20.5 6H10.5C9.75 6 9.25 5.625 8.75 5.25C8.25 4.875 7.75 4.5 7 4.5H4Z M4.30309 9.79403V8.27273H11.4707V9.79403H8.79883V17H6.97496V9.79403H4.30309ZM18.0396 11.0938C17.9799 10.8864 17.8961 10.7031 17.7882 10.544C17.6802 10.3821 17.5481 10.2457 17.3919 10.1349C17.2385 10.0213 17.0623 9.93466 16.8635 9.875C16.6674 9.81534 16.4501 9.78551 16.2115 9.78551C15.7654 9.78551 15.3734 9.89631 15.0353 10.1179C14.7001 10.3395 14.4387 10.6619 14.2512 11.0852C14.0637 11.5057 13.97 12.0199 13.97 12.6278C13.97 13.2358 14.0623 13.7528 14.247 14.179C14.4316 14.6051 14.693 14.9304 15.0311 15.1548C15.3691 15.3764 15.7683 15.4872 16.2285 15.4872C16.6461 15.4872 17.0027 15.4134 17.2981 15.2656C17.5964 15.1151 17.8237 14.9034 17.9799 14.6307C18.139 14.358 18.2186 14.0355 18.2186 13.6634L18.5936 13.7188H16.3436V12.3295H19.9956V13.429C19.9956 14.196 19.8336 14.8551 19.5098 15.4062C19.1859 15.9545 18.7399 16.3778 18.1717 16.6761C17.6035 16.9716 16.9529 17.1193 16.22 17.1193C15.4018 17.1193 14.6831 16.9389 14.0637 16.5781C13.4444 16.2145 12.9615 15.6989 12.6149 15.0312C12.2711 14.3608 12.0993 13.5653 12.0993 12.6449C12.0993 11.9375 12.2015 11.3068 12.4061 10.7528C12.6135 10.196 12.9032 9.72443 13.2754 9.33807C13.6475 8.9517 14.0808 8.65767 14.5751 8.45597C15.0694 8.25426 15.6049 8.15341 16.1816 8.15341C16.676 8.15341 17.1362 8.22585 17.5623 8.37074C17.9885 8.51278 18.3663 8.71449 18.6958 8.97585C19.0282 9.23722 19.2995 9.5483 19.5098 9.90909C19.72 10.267 19.8549 10.6619 19.9146 11.0938H18.0396Z";

        InitializeComponent();
        Nodes = new ObservableCollection<Node>();
        TreeView.ItemsSource = Nodes;
        ProjectsService.Instance.CurrentChanged += FullUpdate;
        // Update();
    }

    public void FullUpdate(Project? project = null)
    {
        Nodes.Clear();
        var path = (project ?? ProjectsService.Instance.Current).Path;
        try
        {
            Nodes.Add(new DirectoryNode(new DirectoryInfo(path)));
        }
        catch (ArgumentException)
        {
        }
    }

    public void Update(Project? project = null)
    {
        if (Nodes.Count == 0)
        {
            var path = (project ?? ProjectsService.Instance.Current).Path;
            try
            {
                Nodes.Add(new DirectoryNode(new DirectoryInfo(path)));
            }
            catch (ArgumentException)
            {
            }
        }
        else
        {
            foreach (var node in Nodes)
            {
                node.Update();
            }
        }
    }

    private void TreeView_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var item = TreeView.SelectedItem;
        if (item is FileNode)
        {
            var fileNode = (FileNode)item;
            AAppService.Instance.Request<string?>("openFile", fileNode.Info.FullName);
        }
    }

    private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e)
    {
        var border = sender as Border;
        if (border == null)
            return;
        var item = TreeView.SelectedItem as Node;
        if (item == null)
            return;

        var openMenu = border.ContextMenu?.Items[0] as MenuItem;
        openMenu?.Items.Clear();
        if (item is FileNode)
        {
            foreach (var provider in CodeTab.Instance?.Providers ?? [])
            {
                if (provider.CanOpen(item.Path))
                {
                    var menuItem = new MenuItem { Header = provider.Name };
                    menuItem.Click += (o, args) => AAppService.Instance.Request<bool>("openFileWith",
                        new OpenFileWithModel { Path = item.Path, ProviderKey = provider.Key });
                    openMenu?.Items.Add(menuItem);
                }
            }

            while ((border.ContextMenu?.Items[3] as Separator)?.Name != "AfterActionsSeparator")
            {
                border.ContextMenu?.Items.RemoveAt(3);
            }

            var flag = false;
            foreach (var action in FileActions)
            {
                if (action.CanUse(item.Path))
                {
                    var menuItem = new MenuItem { Header = action.Name };
                    menuItem.Click += (o, args) => RunAction(action, item.Path);
                    border.ContextMenu?.Items.Insert(3, menuItem);
                    flag = true;
                }
            }

            if (flag)
            {
                border.ContextMenu?.Items.Insert(3, new Separator());
            }
        }

        var createMenu = border.ContextMenu?.Items[2] as MenuItem;
        createMenu?.Items.Clear();
        foreach (var creator in BuiltinFileCreators)
        {
            var menuItem = new MenuItem
                { Header = creator.Name, Icon = new PathIcon { Data = PathGeometry.Parse(creator.Icon ?? "") } };
            menuItem.Click += (o, args) => CreateFile(SelectedItem?.Path, creator);
            createMenu?.Items.Add(menuItem);
        }

        if (FileCreators.Count > 0)
            createMenu?.Items.Add(new Separator());
        foreach (var creator in FileCreators.OrderBy(c => c.Priority))
        {
            var menuItem = new MenuItem
                { Header = creator.Name, Icon = new PathIcon { Data = PathGeometry.Parse(creator.Icon ?? "") } };
            menuItem.Click += (o, args) => CreateFile(SelectedItem?.Path, creator);
            createMenu?.Items.Add(menuItem);
        }
    }

    private async void RunAction(IFileAction action, string path)
    {
        if (action.CreateWindow)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.MainWindow != null)
            {
                var window = new ActionWindow();
                window.Run(action, path);
                await window.ShowDialog(desktop.MainWindow);
            }
        }
        else
        {
            await action.Run(path);
        }
    }

    private async void CreateFile(string? root, IFileCreator creator)
    {
        if (root == null)
            return;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow != null)
        {
            var settingsControl = creator.GetSettingsControl();
            var window = settingsControl == null
                ? CreateFileDialog.Default()
                : new CreateFileDialog(settingsControl);
            window.Confirmed += (options) =>
            {
                creator.Create(root, options);
                Update();
            };
            await window.ShowDialog(desktop.MainWindow);
        }
    }

    private List<Node> SelectedItems
    {
        get
        {
            var items = new List<Node>();
            foreach (var obj in TreeView.SelectedItems)
            {
                if (obj is Node item)
                    items.Add(item);
            }

            return items;
        }
    }

    private Node? SelectedItem => TreeView.SelectedItem as Node;

    private async void Copy_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
        {
            var obj = new DataObject();
            obj.Set(DataFormats.FileNames, SelectedItems.Select(i => i.Path).ToList());
            await clipboard.SetDataObjectAsync(obj);
        }
    }

    private async void CopyPath_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null && SelectedItem != null)
        {
            await clipboard.SetTextAsync(SelectedItem.Path);
        }
    }

    private async void Paste_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
        {
            var root = SelectedItem?.Path ?? "";
            if (File.Exists(root))
                root = Path.GetDirectoryName(root);
            var data = await clipboard.GetDataAsync(DataFormats.FileNames);
            foreach (var path in data as List<string> ?? [])
            {
                File.Copy(path, Path.Join(root, Path.GetFileName(path)));
            }

            Update();
        }
    }
}