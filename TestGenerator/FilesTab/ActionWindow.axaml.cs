using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.FilesTab;

public partial class ActionWindow : Window
{
    private Thread? _thread;

    public ActionWindow()
    {
        InitializeComponent();
    }

    public async void Run(IFileAction action, string path)
    {
        Title = action.Name;

        try
        {
            await Task.Run(() => action.Run(path));
            // _thread = new Thread(() =>
            // {
            //     Console.WriteLine(1);
            //     var task = action.Run(path);
            //     task.Wait();
            //     Console.WriteLine(3);
            // });
            // _thread.Start();
            // await Task.Run(_thread.Join);
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
        _thread?.Interrupt();
        Close();
    }
}