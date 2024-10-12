using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Threading;

namespace TestGenerator.UI;

public partial class Terminal : UserControl
{
    protected Process? CurrentProcess;
    public string CurrentDirectory { get; set; } = ".";
    public string TerminalApp { get; set; } = "powershell";
    public string TerminalAppArgs { get; set; } = "-Command";

    public Terminal()
    {
        InitializeComponent();
        WritePrompt();
    }

    private async void Box_OnReturn(string command)
    {
        var text = Box.GetInput();
        Write("\n");
        if (CurrentProcess == null)
            await RunProcess(text);
        else
        {
            CurrentProcess.StandardInput.WriteLine(text);
        }
    }

    protected void Write(string? text)
    {
        Box.Write(text);
    }

    public void Clear()
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

    protected async Task<Process?> RunProcess(string? command)
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            if (command.Trim() == "clear")
                Clear();
            else if (command.Trim().StartsWith("cd "))
                ChangeDirectory(command.Substring(2).Trim());
            else
            {
                var proc = CurrentProcess = new Process();
                CurrentProcess.StartInfo.FileName = TerminalApp;
                CurrentProcess.StartInfo.Arguments = TerminalAppArgs + " " + command;
                CurrentProcess.StartInfo.WorkingDirectory = CurrentDirectory;
                CurrentProcess.StartInfo.CreateNoWindow = true;
                CurrentProcess.StartInfo.StandardInputEncoding = Encoding.UTF8;
                CurrentProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                CurrentProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                CurrentProcess.StartInfo.UseShellExecute = false;
                CurrentProcess.StartInfo.RedirectStandardOutput = true;
                CurrentProcess.StartInfo.RedirectStandardInput = true;
                CurrentProcess.StartInfo.RedirectStandardError = true;
                try
                {
                    CurrentProcess.Start();
                    CurrentProcess.OutputDataReceived += CurrentProcessOnOutputDataReceived;
                    CurrentProcess.Exited += CurrentProcessOnExited;
                    CurrentProcess.BeginOutputReadLine();

                    await CurrentProcess.WaitForExitAsync();
                }
                catch (Exception e)
                {
                    Write(e.Message + "\n");
                }

                CurrentProcess = null;
                return proc;
            }
        }

        WritePrompt();
        return null;
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