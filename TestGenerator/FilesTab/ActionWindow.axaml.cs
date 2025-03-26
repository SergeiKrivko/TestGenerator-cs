using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.FilesTab;

public partial class ActionWindow : Window
{
    public ActionWindow()
    {
        InitializeComponent();
    }

    internal async void Run(IFileAction action, string path)
    {
        ActionNameBlock.Text = action.Name;

        try
        {
            await Task.Run(() => action.Run(path));
            Spinner.IsVisible = false;
            SuccessPanel.IsVisible = true;
            ButtonCancel.IsVisible = false;
            ButtonOk.IsVisible = true;
            await Task.Delay(1000);
            Close();
        }
        catch (Exception e)
        {
            LogService.Logger.Error($"Error in file action '{action.Key}' with '{path}': {e.Message}");
            Spinner.IsVisible = false;
            ErrorBlock.Text = e.Message;
            ErrorBlock.IsVisible = true;
            ButtonCancel.IsVisible = false;
            ButtonOk.IsVisible = true;
        }
    }

    private void ButtonOk_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ButtonCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}