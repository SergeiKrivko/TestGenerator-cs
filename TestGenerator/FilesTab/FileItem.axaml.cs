using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using TestGenerator.MainTabs.Code;
using TestGenerator.Shared.Types;

namespace TestGenerator.FilesTab;

public partial class FileItem : UserControl
{
    public static readonly StyledProperty<Node?>
        NodeProperty = AvaloniaProperty.Register<FileItem, Node?>(nameof(Node));

    public Node? Node
    {
        get => GetValue(NodeProperty);
        set => SetValue(NodeProperty, value);
    }

    public event Action<Node>? CopyRequested; 
    public event Action<Node>? PasteRequested; 
    public event Action<Node>? DeleteRequested; 
    public event Action<Node>? SendToTrashRequested; 
    public event Action? GlobalUpdateRequested; 

    public FileItem()
    {
        InitializeComponent();
        PropertyChanged += (sender, args) =>
        {
            if (args.Property == NodeProperty)
            {
                IconBlock.Data = PathGeometry.Parse(Node?.Icon ?? "");
                FilenameBlock.Text = Node?.Title;
            }
        };
    }

    private void Copy_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Node != null)
            CopyRequested?.Invoke(Node);
    }

    private async void CopyPath_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null && Node != null)
        {
            await clipboard.SetTextAsync(Node.Path);
        }
    }

    private void Paste_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Node != null)
            PasteRequested?.Invoke(Node);
    }

    private void UpdateOpenMenu()
    {
        OpenMenu.Items.Clear();
        if (Node is not FileNode)
            return;
        foreach (var provider in CodeTab.Instance?.Providers ?? [])
        {
            try
            {
                if (provider.CanOpen(Node.Path))
                {
                    var menuItem = new MenuItem { Header = provider.Name };
                    menuItem.Click += (o, args) => AAppService.Instance.Request<bool>("openFileWith",
                        new OpenFileWithModel { Path = Node.Path, ProviderKey = provider.Key });
                    OpenMenu.Items.Add(menuItem);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }
        }
    }

    private void UpdateActionsMenu()
    {
        if (Node == null)
            return;
        while ((ContextMenu.Items[3] as Separator)?.Name != "AfterActionsSeparator")
        {
            ContextMenu.Items.RemoveAt(3);
        }

        var flag = false;
        foreach (var action in FilesTab.FileActions)
        {
            if (action.CanUse(Node.Path))
            {
                var menuItem = new MenuItem { Header = action.Name };
                menuItem.Click += (o, args) => RunAction(action, Node.Path);
                ContextMenu.Items.Insert(3, menuItem);
                flag = true;
            }
        }

        if (flag)
        {
            ContextMenu.Items.Insert(3, new Separator());
        }
    }

    private static async void RunAction(IFileAction action, string path)
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

    private void UpdateCreateMenu()
    {
        if (Node == null)
            return;
        
        foreach (var creator in FilesTab.BuiltinFileCreators)
        {
            var menuItem = new MenuItem
                { Header = creator.Name, Icon = new PathIcon { Data = PathGeometry.Parse(creator.Icon ?? "") } };
            menuItem.Click += (o, args) => CreateFile(Node.Path, creator);
            CreateMenu.Items.Add(menuItem);
        }

        if (FilesTab.FileCreators.Count > 0)
            CreateMenu.Items.Add(new Separator());
        foreach (var creator in FilesTab.FileCreators.OrderBy(c => c.Priority).Where(c => c.Enabled))
        {
            var menuItem = new MenuItem
                { Header = creator.Name, Icon = new PathIcon { Data = PathGeometry.Parse(creator.Icon ?? "") } };
            menuItem.Click += (o, args) => CreateFile(Node.Path, creator);
            CreateMenu.Items.Add(menuItem);
        }
    }

    private async void CreateFile(string? root, IFileCreator creator)
    {
        if (root == null)
            return;
        if (File.Exists(root))
            root = Path.GetDirectoryName(root) ?? "";
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
                GlobalUpdateRequested?.Invoke();
            };
            await window.ShowDialog(desktop.MainWindow);
        }
    }

    private bool _contextMenuCreated;

    private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e)
    {
        if (_contextMenuCreated)
            return;
        _contextMenuCreated = true;
        UpdateOpenMenu();
        UpdateActionsMenu();
        UpdateCreateMenu();
    }

    private void Delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Node == null)
            return;
        DeleteRequested?.Invoke(Node);
    }

    private void SentToTrash_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Node == null)
            return;
        SendToTrashRequested?.Invoke(Node);
    }
}