using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Avalonia.Controls;
using Avalonia.Threading;

namespace TestGenerator.UI;

public partial class Terminal : UserControl
{
    private Process? _currentProcess;
    public string CurrentDirectory { get; set; } = ".";
    public string TerminalApp { get; set; } = "powershell";
    public string TerminalAppArgs { get; set; } = "-Command";

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

    private void Clear()
    {
        Box.ClearText();
    }

    private void ChangeDirectory(string directory)
    {
        if (Path.IsPathFullyQualified(directory))
            CurrentDirectory = directory.Trim();
        else
        {
            CurrentDirectory = Path.GetFullPath(Path.Combine(CurrentDirectory, directory));
        }
    }

    private async void RunProcess(string? command)
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            if (command.Trim() == "clear")
                Clear();
            else if (command.Trim().StartsWith("cd "))
                ChangeDirectory(command.Substring(2).Trim());
            else
            {
                _currentProcess = new Process();
                _currentProcess.StartInfo.FileName = TerminalApp;
                _currentProcess.StartInfo.Arguments = TerminalAppArgs + " " + command;
                _currentProcess.StartInfo.WorkingDirectory = CurrentDirectory;
                _currentProcess.StartInfo.CreateNoWindow = true;
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
                return;
            }
        }

        WritePrompt();
    }

    private void CurrentProcessOnExited(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() => { WritePrompt(); });
    }

    private void CurrentProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Dispatcher.UIThread.Post(() => { Write(e.Data + "\n"); });
    }

    protected virtual string Prompt => Path.GetFullPath(CurrentDirectory) + "> ";

    protected void WritePrompt()
    {
        Write(Prompt);
    }
}