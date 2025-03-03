using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using AvaluxUI.Utils;
using Microsoft.VisualBasic.FileIO;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.FilesTab;

public partial class FilesTab : SideTab
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();
    
    private ObservableCollection<Node> Nodes { get; }

    public override string TabName => "Файлы";
    public override string TabKey => "Files";

    public override string TabIcon =>
        "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z";

    public static List<IFileCreator> BuiltinFileCreators { get; } = [new FileCreator(), new DirectoryCreator()];
    public static List<IFileCreator> FileCreators { get; } = [];
    public static List<IFileAction> FileActions { get; } = [];

    public FilesTab()
    {
        Node.RegexIcons[new Regex(@"[/\\]\.TestGenerator$")] =
            "M1 6C1 3 4 3 4 3H7.5C8 3 8.5 3.375 9 3.75C9.5 4.125 10 4.5 10.5 4.5H20.5C23.5 4.5 23.5 7.5 23.5 7.5V18C23.5 21.1354 20.5 21 20.5 21H4C4 21 1 21 1 18V6ZM4 4.5C4 4.5 2.5 4.5 2.5 6V18C2.5 18 2.5 19.5 4 19.5H20.5C20.5 19.5 22 19.5 22 18V7.5C22 7.5 22 6 20.5 6H10.5C9.75 6 9.25 5.625 8.75 5.25C8.25 4.875 7.75 4.5 7 4.5H4Z M4.30309 9.79403V8.27273H11.4707V9.79403H8.79883V17H6.97496V9.79403H4.30309ZM18.0396 11.0938C17.9799 10.8864 17.8961 10.7031 17.7882 10.544C17.6802 10.3821 17.5481 10.2457 17.3919 10.1349C17.2385 10.0213 17.0623 9.93466 16.8635 9.875C16.6674 9.81534 16.4501 9.78551 16.2115 9.78551C15.7654 9.78551 15.3734 9.89631 15.0353 10.1179C14.7001 10.3395 14.4387 10.6619 14.2512 11.0852C14.0637 11.5057 13.97 12.0199 13.97 12.6278C13.97 13.2358 14.0623 13.7528 14.247 14.179C14.4316 14.6051 14.693 14.9304 15.0311 15.1548C15.3691 15.3764 15.7683 15.4872 16.2285 15.4872C16.6461 15.4872 17.0027 15.4134 17.2981 15.2656C17.5964 15.1151 17.8237 14.9034 17.9799 14.6307C18.139 14.358 18.2186 14.0355 18.2186 13.6634L18.5936 13.7188H16.3436V12.3295H19.9956V13.429C19.9956 14.196 19.8336 14.8551 19.5098 15.4062C19.1859 15.9545 18.7399 16.3778 18.1717 16.6761C17.6035 16.9716 16.9529 17.1193 16.22 17.1193C15.4018 17.1193 14.6831 16.9389 14.0637 16.5781C13.4444 16.2145 12.9615 15.6989 12.6149 15.0312C12.2711 14.3608 12.0993 13.5653 12.0993 12.6449C12.0993 11.9375 12.2015 11.3068 12.4061 10.7528C12.6135 10.196 12.9032 9.72443 13.2754 9.33807C13.6475 8.9517 14.0808 8.65767 14.5751 8.45597C15.0694 8.25426 15.6049 8.15341 16.1816 8.15341C16.676 8.15341 17.1362 8.22585 17.5623 8.37074C17.9885 8.51278 18.3663 8.71449 18.6958 8.97585C19.0282 9.23722 19.2995 9.5483 19.5098 9.90909C19.72 10.267 19.8549 10.6619 19.9146 11.0938H18.0396Z";
        Node.RegexIcons[new Regex(@"[/\\](\w*\.)?[Mm][Aa][Kk][Ee][Ff][Ii][Ll][Ee](\.\w+)?$")] =
            "M4.11381 4.45455H7.90643L11.9121 14.2273H12.0826L16.0882 4.45455H19.8809V19H16.8979V9.53267H16.7772L13.013 18.929H10.9817L7.21751 9.49716H7.09677V19H4.11381V4.45455Z";

        InitializeComponent();
        Nodes = new ObservableCollection<Node>();
        TreeView.ItemsSource = Nodes;
        _projectsService.CurrentChanged += FullUpdate;
        // Update();
    }

    public void FullUpdate(IProject? project = null)
    {
        Nodes.Clear();
        var path = (project ?? _projectsService.Current).Path;
        try
        {
            var node = new DirectoryNode(new DirectoryInfo(path));
            node.Update(recurseLevel: 2);
            Nodes.Add(node);
        }
        catch (ArgumentException)
        {
        }
    }

    public void Update(Project? project = null)
    {
        if (Nodes.Count == 0)
        {
            var path = (project ?? _projectsService.Current).Path;
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
                node.UpdateIfExpanded();
            }
        }
    }

    private async void TreeView_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var item = TreeView.SelectedItem;
        if (item is FileNode)
        {
            var fileNode = (FileNode)item;
            await _appService.Request<string?>("openFile", fileNode.Info.FullName);
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

    private async void FileItem_OnCopyRequested(Node node)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null)
            return;
        var obj = new DataObject();
        obj.Set(DataFormats.FileNames, SelectedItems.Select(i => i.Path).ToArray());
        await clipboard.SetDataObjectAsync(obj);
    }

    private async void FileItem_OnPasteRequested(Node node)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
        {
            var root = node.Path;
            if (File.Exists(root))
                root = Path.GetDirectoryName(root);
            var data = await clipboard.GetDataAsync(DataFormats.FileNames);
            foreach (var path in data as List<string> ?? [])
            {
                var dst = Path.Join(root, Path.GetFileName(path));
                var i = 1;
                while (File.Exists(dst))
                    dst = Path.Join(root,
                        Path.GetFileNameWithoutExtension(path) + $" ({i++})" + Path.GetExtension(path));
                File.Copy(path, dst);
            }

            Update();
        }
    }

    private void FileItem_OnGlobalUpdateRequested()
    {
        Update();
    }

    private void FileItem_OnDeleteRequested(Node node)
    {
        foreach (var item in TreeView.SelectedItems)
        {
            if (item is FileNode fileNode)
            {
                File.Delete(fileNode.Path);
            }
            else if (item is DirectoryNode directoryNode)
            {
                Directory.Delete(directoryNode.Path, recursive: true);
            }
        }

        Update();
    }

    private async void FileItem_OnSentToTrashRequested(Node node)
    {
        foreach (var item in TreeView.SelectedItems)
        {
            if (item is FileNode fileNode)
            {
                if (OperatingSystem.IsWindows())
                    await Task.Run(() =>
                        FileSystem.DeleteFile(fileNode.Path, UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin));
                else
                {
                    await _appService.RunProcess(new RunProcessArgs
                        { Filename = "bash", Args = $"gio trash \"{fileNode.Path}\"" });
                }
            }
            else if (item is DirectoryNode directoryNode)
            {
                if (OperatingSystem.IsWindows())
                    await Task.Run(() =>
                        FileSystem.DeleteDirectory(directoryNode.Path, UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin));
                else
                {
                    await _appService.RunProcess(new RunProcessArgs
                        { Filename = "bash", Args = $"gio trash \"{directoryNode.Path}\"" });
                }
            }

            Update();
        }
    }

    private void TreeView_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (TreeView.SelectedItem is Node node)
        {
            if (e.Key == Key.Delete)
            {
                if ((e.KeyModifiers & KeyModifiers.Shift) != 0)
                    FileItem_OnDeleteRequested(node);
                else
                    FileItem_OnSentToTrashRequested(node);
            }

            if ((e.KeyModifiers & KeyModifiers.Control) != 0)
            {
                if (e.Key == Key.C)
                    FileItem_OnCopyRequested(node);
                else if (e.Key == Key.V)
                    FileItem_OnPasteRequested(node);
            }
        }
    }

    private async void FileItem_OnDragRequested(Node obj, PointerEventArgs e)
    {
        var data = new DataObject();
        data.Set(DataFormats.FileNames, SelectedItems.Select(i => i.Path).ToArray());
        await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
    }
}