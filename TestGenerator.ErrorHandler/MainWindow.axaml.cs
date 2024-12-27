using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TestGenerator.ErrorHandler;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Load();
    }
    
    private async Task<string> ReadLogFile()
    {
        while (true)
        {
            try
            {
                return await File.ReadAllTextAsync(Program.LogFilePath ?? "");
            }
            catch (IOException)
            {
                await Task.Delay(200);
            }
        }
    }

    private async void Load()
    {
        LogBlock.Text = await ReadLogFile();
    }

    private async void ButtonSend_OnClick(object? sender, RoutedEventArgs e)
    {
        var client = new HttpClient();
        await client.PostAsync("https://testgenerator-api.nachert.art/api/v1/logs", 
            JsonContent.Create(LogBlock.Text ?? ""));
        Close();
    }

    private void ButtonNotSend_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}