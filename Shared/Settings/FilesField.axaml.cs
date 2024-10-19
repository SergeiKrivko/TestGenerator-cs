using Avalonia.Controls;
using Shared.Settings.Shared;
using Shared.Utils;

namespace Shared.Settings;

public partial class FilesField : UserControl, IField
{
    public object? Value
    {
        get => (_topLevelItem?.Current ?? []).Select(p => System.IO.Path.GetRelativePath(Path, p));
        set => _topLevelItem?.Select((value as string[] ?? []).Select(p => System.IO.Path.Join(Path, p)));
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
        Panel.Children.Add(_topLevelItem);
    }

    public void Load(SettingsSection section)
    {
        if (Key != null)
            Value = section.Get<string>(Key);
    }
}