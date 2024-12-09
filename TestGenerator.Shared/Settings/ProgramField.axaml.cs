using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using TestGenerator.Shared.SidePrograms;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Settings;

public partial class ProgramField : UserControl, IField
{
    public object? Value
    {
        get => (ComboBox.SelectedItem as SideProgramFile)?.ToModel();
        set
        {
            if (value is ProgramFileModel model)
            {
                ComboBox.SelectedItem =
                    Items.FirstOrDefault(i => i.Path == model.Path && i.VirtualSystem.Key == model.VirtualSystem);
                if (ComboBox.SelectedItem == null)
                {
                    var program = Program.FromModel(model);
                    if (program != null)
                    {
                        program.ValidateAsync();
                        Items.Add(program);
                        ComboBox.SelectedItem = program;
                    }
                }
            }
            else
            {
                ComboBox.SelectedItem = null;
            }
            
        }
    }

    public required SideProgram Program { get; init; }

    public ObservableCollection<SideProgramFile> Items { get; } = [];

    public string? Key { get; set; }

    public event IField.ChangeHandler? ValueChanged;

    public string? FieldName
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public bool Directory { get; set; } = false;

    public string? Extension { get; set; }

    public ProgramField()
    {
        InitializeComponent();
        ComboBox.ItemsSource = Items;
    }

    private async Task Search()
    {
        await Task.Delay(100);
        foreach (var programFile in await Program.Search())
        {
            Items.Add(programFile);
        }
    }

    public async void Load(SettingsSection section)
    {
        if (Key != null)
        {
            var value = section.Get<ProgramFileModel>(Key);
            await Search();
            Value = value;
        }
    }

    private void ButtonEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        SetEditActive(true);
        TextBox.Focus();
    }

    private void ButtonCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        SetEditActive(false);
    }

    private void SetEditActive(bool active)
    {
        ButtonEdit.IsVisible = !active;
        ButtonCancel.IsVisible = active;
        ComboBox.IsVisible = !active;
        TextBox.IsVisible = active;
    }

    private async void TextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Return)
        {
            SetEditActive(false);
            var program = Program.FromPath(TextBox.Text ?? "");
            await program.Validate();
            Items.Add(program);
            ComboBox.SelectedItem = program;
        }
    }

    private void TextBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        SetEditActive(false);
    }

    private void ComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, Value);
    }
}