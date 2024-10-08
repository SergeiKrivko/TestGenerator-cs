using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using InvalidOperationException = System.InvalidOperationException;

namespace TestGenerator.UI;

public partial class Terminal : UserControl
{
    private Process? _currentProcess;
    public string CurrentDirectory { get; set; } = ".";
    public string LastText = "";
    
    public Terminal()
    {
        InitializeComponent();
        WritePrompt();
    }

    private void Box_OnReturn(string command)
    {
        var text = Box.GetInput();
        Write("\n");
        if (_currentProcess == null)
            RunProcess(text);
        else
        {
            _currentProcess.StandardInput.WriteLine(text);
        }
    }

    protected void Write(string? text)
    {
        Box.Write(text);
    }

    private async void RunProcess(string? command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            WritePrompt();
            return;
        }
        var arguments = "";
        if (command.Contains(' '))
        {
            arguments = command.Substring(command.IndexOf(' '));
            command = command.Substring(0, command.IndexOf(' '));
        }
        
        _currentProcess = new Process();
        _currentProcess.StartInfo.FileName = command;
        _currentProcess.StartInfo.Arguments = arguments;
        _currentProcess.StartInfo.WorkingDirectory = CurrentDirectory;
        _currentProcess.StartInfo.StandardInputEncoding = Encoding.UTF8;
        _currentProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;
        _currentProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        _currentProcess.StartInfo.UseShellExecute = false;
        _currentProcess.StartInfo.RedirectStandardOutput = true;
        _currentProcess.StartInfo.RedirectStandardInput = true;
        _currentProcess.StartInfo.RedirectStandardError = true;
        try
        {
            _currentProcess.Start();
            _currentProcess.OutputDataReceived += CurrentProcessOnOutputDataReceived;
            _currentProcess.Exited += CurrentProcessOnExited;
            _currentProcess.BeginOutputReadLine();
            
            await _currentProcess.WaitForExitAsync();
        }
        catch (Exception e)
        {
            Write(e.Message + "\n");
        }

        _currentProcess = null;
        WritePrompt();
    }

    private void CurrentProcessOnExited(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            WritePrompt();
        });
    }

    private void CurrentProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Write(e.Data + "\n");
        });
    }

    protected virtual string Prompt => Path.GetFullPath(CurrentDirectory) + "> ";

    protected void WritePrompt()
    {
        Write(Prompt);
    }
}