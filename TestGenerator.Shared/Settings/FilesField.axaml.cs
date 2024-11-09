using Avalonia.Controls;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Utils;

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

    private DirectoryItem? _topLevelItem = null;

    public event IField.ChangeHandler? ValueChanged;

    public FilesField(string path, string[] extensions)
    {
        Path = path;
        Extensions = extensions;
        InitializeComponent();
        Update();
    }

    private void Update()
    {
        Panel.Children.Clear();
        _topLevelItem = new DirectoryItem(Path, Extensions);
        _topLevelItem.Changed += () => { ValueChanged?.Invoke(this, Value); };
        Panel.Children.Add(_topLevelItem);
    }

    public void Load(SettingsSection section)
    {
        if (Key != null)
            Value = section.Get<string[]>(Key);
    }
}