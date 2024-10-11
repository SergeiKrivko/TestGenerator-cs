using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Shared.Utils;

namespace Shared.Settings;

public partial class PathField : UserControl, IField
{
    public object? Value
    {
        get => TextBox.Text;
        set => TextBox.Text = value?.ToString();
    }

    public string? Key { get; set; }

    public event IField.ChangeHandler? ValueChanged;

    public string? FieldName
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public bool Directory { get; set; } = false;
    
    public string? Extension { get; set; }
    
    public PathField()
    {
        InitializeComponent();
    }

    public void Load(SettingsSection section)
    {
        if (Key != null)
            Value = section.Get<string>(Key);
    }

    private void TextBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        ValueChanged?.Invoke(this, TextBox.Text);
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        if (Directory)
        {
            var files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Выберите папку",
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                TextBox.Text = files[0].Path.AbsolutePath;
                ValueChanged?.Invoke(this, TextBox.Text);
            }
        }
        else
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выберите файл",
                FileTypeFilter = Extension == null ? [] : [new FilePickerFileType(Extension)],
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                TextBox.Text = files[0].Path.AbsolutePath;
                ValueChanged?.Invoke(this, TextBox.Text);
            }
        }
    }
}