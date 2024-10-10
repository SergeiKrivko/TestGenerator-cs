using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Core.Services;
using Core.Types;
using Shared;

namespace TestGenerator.FilesTab;

public partial class FilesTab : UserControl
{
    public ObservableCollection<Node> Nodes{ get; }
    
    public FilesTab()
    {
        InitializeComponent();
        Nodes = new ObservableCollection<Node>();
        TreeView.ItemsSource = Nodes;
        ProjectsService.Instance.CurrentChanged += Update;
        // Update();
    }

    public void Update(Project? project = null)
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

    private void TreeView_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var item = TreeView.SelectedItem;
        if (item is FileNode)
        {
            var fileNode = (FileNode)item;
            AAppService.Instance.MainTabShow("Code");
            AAppService.Instance.MainTabCommand("Code", "open", fileNode.Info.FullName);
        }
    }
}

public class Node
{
    public ObservableCollection<Node> SubNodes { get; }
    public string Title { get; }
    public string Icon { get; set; } = "";
  
    public Node(string title)
    {
        Title = title;
        SubNodes = new ObservableCollection<Node>();
    }

    public Node(string title, ObservableCollection<Node> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
    }
}

class DirectoryNode : Node
{
    public DirectoryInfo Info { get; }

    public DirectoryNode(DirectoryInfo info): base(info.Name)
    {
        Info = info;
        Update();
        Icon = "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z";
    }

    public void Update()
    {
        SubNodes.Clear();
        foreach (var elem in Info.GetDirectories())
        {
            SubNodes.Add(new DirectoryNode(elem));
        }
        foreach (var elem in Info.GetFiles())
        {
            SubNodes.Add(new FileNode(elem));
        }
    }
}

class FileNode : Node
{
    public FileInfo Info { get; }

    public FileNode(FileInfo info): base(info.Name)
    {
        Info = info;
    }
}