using System.Collections.ObjectModel;
using Avalonia.Controls;
using AvaluxUI.Utils;
using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Shared.Settings;

public partial class FilesField : UserControl, IField
{
    public object? Value
    {
        get => (_topLevelItem?.Current ?? []).Select(p => System.IO.Path.GetRelativePath(Path, p).Replace('\\', '/'));
        set => _topLevelItem?.Select((value as IEnumerable<string> ?? [])
            .Select(p => System.IO.Path.GetFullPath(System.IO.Path.Join(Path, p))).ToArray());
    }

    public string? Key { get; set; }

    public string[] Extensions { get; }
    public string Path { get; }

    private ObservableCollection<INode> Nodes { get; set; } = [];

    private DirectoryNode? _topLevelItem;

    public event IField.ChangeHandler? ValueChanged;

    public FilesField(string path, string[] extensions)
    {
        Path = path;
        Extensions = extensions;
        InitializeComponent();
        Tree.ItemsSource = Nodes;
        Update();
    }

    private void Update()
    {
        _topLevelItem = new DirectoryNode(Path, Extensions);
        _topLevelItem.Changed += () => { ValueChanged?.Invoke(this, Value); };
        Nodes.Clear();
        foreach (var node in _topLevelItem.Children)
        {
            Nodes.Add(node);
        }
    }

    public void Load(ISettingsSection section)
    {
        if (Key != null)
            Value = section.Get<string[]>(Key);
    }
}