using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestGenerator.FilesTab;

public abstract class Node
{
    public ObservableCollection<Node> SubNodes { get; }
    public string Title { get; }
    public string Path { get; }
    public string? Icon { get; set; }
    public static Dictionary<Regex, string> RegexIcons { get; } = [];

    protected Node(string path, string title)
    {
        Path = path;
        Title = title;
        SubNodes = new ObservableCollection<Node>();
        Icon = GetRegexIcon();
    }

    protected Node(string path, string title, ObservableCollection<Node> subNodes)
    {
        Path = path;
        Title = title;
        SubNodes = subNodes;
        Icon = GetRegexIcon();
    }

    private string? GetRegexIcon()
    {
        foreach (var regexIcon in RegexIcons)
        {
            if (regexIcon.Key.IsMatch(Path))
                return regexIcon.Value;
        }

        return null;
    }

    public abstract void Update(int? recurseLevel = null);

    public void UpdateIfExpanded()
    {
        if (IsExpanded)
            Update();
        foreach (var node in SubNodes)
        {
            node.UpdateIfExpanded();
        }
    }

    public abstract bool Exists { get; }
    
    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            if (_isExpanded)
                Update(recurseLevel: 2);
        }
    }
}