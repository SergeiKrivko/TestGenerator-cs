﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class Terminal : UserControl
{
    protected Process? CurrentProcess;
    public string CurrentDirectory { get; set; } = ".";
    private string TerminalApp { get; }
    private string TerminalAppArgs { get; }

    private bool TerminalQuotes { get; }

    private readonly List<string> _lastCommands = [];
    private int _lastCommandIndex = 0;

    public Terminal()
    {
        InitializeComponent();
        WritePrompt();
        if (OperatingSystem.IsWindows())
        {
            TerminalApp = "powershell";
            TerminalAppArgs = "-Command";
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            TerminalApp = "bash";
            TerminalAppArgs = "-c";
            TerminalQuotes = true;
        }
        else
        {
            LogService.Logger.Warning("Terminal: Unknown operating system");
            TerminalApp = "";
            TerminalAppArgs = "";
        }
    }

    private async void Box_OnReturn(string command)
    {
        // var text = Box.GetInput();
        Write("\n");
        if (CurrentProcess == null)
            await RunProcess(command);
        else
        {
            await CurrentProcess.StandardInput.WriteLineAsync(command);
            await CurrentProcess.StandardInput.FlushAsync();
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
        CurrentDirectory = Path.IsPathFullyQualified(directory)
            ? directory.Trim()
            : Path.GetFullPath(Path.Combine(CurrentDirectory, directory));
    }

    private void PreviousCommand()
    {
        if (CurrentProcess == null && _lastCommandIndex > 0)
        {
            _lastCommandIndex -= 1;
            Box.Rewrite(_lastCommands[_lastCommandIndex]);
        }
    }

    private void NextCommand()
    {
        if (CurrentProcess == null && _lastCommandIndex < _lastCommands.Count)
        {
            _lastCommandIndex += 1;
            Box.Rewrite(_lastCommandIndex == _lastCommands.Count ? "" : _lastCommands[_lastCommandIndex]);
        }
    }

    protected virtual async Task<ICompletedProcess?> RunProcess(string? command, string? stdin = null,
        EnvironmentModel? environment = null,
        CancellationToken token = new())
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            _lastCommands.Add(command);
            _lastCommandIndex = _lastCommands.Count;

            if (command.Trim() == "clear")
                Clear();
            else if (command.Trim().StartsWith("cd "))
                ChangeDirectory(command.Substring(2).Trim());
            else
            {
                var proc = CurrentProcess = new Process();
                CurrentProcess.StartInfo.FileName = TerminalApp;
                CurrentProcess.StartInfo.Arguments = TerminalAppArgs + " " +
                                                     (TerminalQuotes ? "\"" + command + "\"" : command);
                CurrentProcess.StartInfo.WorkingDirectory = CurrentDirectory;
                CurrentProcess.StartInfo.CreateNoWindow = true;
                // CurrentProcess.StartInfo.StandardInputEncoding = Encoding.UTF8;
                CurrentProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                CurrentProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                CurrentProcess.StartInfo.UseShellExecute = false;
                CurrentProcess.StartInfo.RedirectStandardOutput = true;
                CurrentProcess.StartInfo.RedirectStandardInput = true;
                CurrentProcess.StartInfo.RedirectStandardError = true;
                if (environment?.InheritGlobal == false)
                    CurrentProcess.StartInfo.Environment.Clear();
                foreach (var variable in environment?.Variables ?? [])
                {
                    CurrentProcess.StartInfo.Environment[variable.Name] = variable.Value;
                }

                try
                {
                    CurrentProcess.Start();
                    ReadOutputLoop();
                    ReadErrorLoop();
                    if (stdin != null)
                    {
                        await CurrentProcess.StandardInput.WriteAsync(stdin);
                        await CurrentProcess.StandardInput.FlushAsync(token);
                    }

                    try
                    {
                        await CurrentProcess.WaitForExitAsync(token);
                    }
                    catch (TaskCanceledException)
                    {
                        LogService.Logger.Debug("Killing process...");
                        try
                        {
                            CurrentProcess.Kill(entireProcessTree: true);
                        }
                        catch (Exception e)
                        {
                            LogService.Logger.Warning($"Error while killing process '{CurrentProcess}': {e.Message}");
                        }

                        // Write(await proc.StandardOutput.ReadToEndAsync(token));
                        // Write(await proc.StandardError.ReadToEndAsync(token));

                        try
                        {
                            CurrentProcess.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.Logger.Warning($"Error while dispose process '{CurrentProcess}': {e.Message}");
                        }

                        CurrentProcess = null;
                        LogService.Logger.Debug("Process terminated");

                        WritePrompt();
                        throw;
                    }

                    CurrentProcess = null;
                    LogService.Logger.Information($"Process {proc.Id} exited with code {proc.ExitCode}");

                    Write(await proc.StandardOutput.ReadToEndAsync(token));
                    Write(await proc.StandardError.ReadToEndAsync(token));
                    WritePrompt();
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Write(e.Message + "\n");
                    CurrentProcess = null;
                }

                return new CompletedProcess
                {
                    ExitCode = proc.ExitCode,
                    Stdout = _stdout,
                    Stderr = _stderr,
                    // Time = proc.TotalProcessorTime
                };
            }
        }

        WritePrompt();
        return null;
    }

    private string _stdout = "";
    private string _stderr = "";

    private async void ReadOutputLoop()
    {
        if (CurrentProcess == null)
            return;
        _stdout = "";
        var pid = CurrentProcess.Id;
        var chars = new Memory<char>(new char[100]);
        while (pid == CurrentProcess?.Id)
        {
            var count = await CurrentProcess.StandardOutput.ReadAsync(chars);
            if (count > 0)
            {
                var el = chars.Slice(0, count).ToString();
                _stdout += el;
                Write(el);
            }

            await Task.Delay(10);
        }
    }

    private async void ReadErrorLoop()
    {
        if (CurrentProcess == null)
            return;
        _stderr = "";
        var pid = CurrentProcess.Id;
        var chars = new Memory<char>(new char[100]);
        while (pid == CurrentProcess?.Id)
        {
            var count = await CurrentProcess.StandardError.ReadAsync(chars);
            if (count > 0)
            {
                var el = chars.Slice(0, count).ToString();
                _stderr += el;
                Write(el);
            }

            await Task.Delay(10);
        }
    }

    protected virtual string Prompt => Path.GetFullPath(CurrentDirectory) + "> ";

    public void WritePrompt()
    {
        Write(Prompt);
    }
}