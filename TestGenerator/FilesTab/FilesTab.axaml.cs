using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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