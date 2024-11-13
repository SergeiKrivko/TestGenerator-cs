using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Utils;

namespace TestGenerator.FilesTab;

public partial class CreateFileDialog : Window
{
    public SettingsControl SettingsControl { get; }
    private readonly SettingsSection _settingsSection = SettingsSection.Empty("");

    public delegate void ConfirmHandler(SettingsSection options);

    public event ConfirmHandler? Confirmed;

    public CreateFileDialog(SettingsControl settingsControl)
    {
        SettingsControl = settingsControl;
        SettingsControl.Section = _settingsSection;
        InitializeComponent();
        SettingsControlPanel.Children.Add(settingsControl);
    }

    public static CreateFileDialog Default()
    {
        var settingsControl = new SettingsControl();
        settingsControl.Add(new StringField { Key = "Name", FieldName = "Имя файла" });
        return new CreateFileDialog(settingsControl);
    }

    public static CreateFileDialog Empty()
    {
        return new CreateFileDialog(new SettingsControl());
    }

    private void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Confirmed?.Invoke(_settingsSection);
        Close();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}