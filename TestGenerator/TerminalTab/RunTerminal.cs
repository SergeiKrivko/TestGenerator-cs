﻿using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Documents;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Types;
using TestGenerator.UI;

namespace TestGenerator.TerminalTab;

public class RunTerminal : Terminal
{
    public RunTerminal()
    {
        Box.IsReadOnly = true;
    }

    protected override string Prompt => "";

    public async Task<ICompletedProcess> Run(RunProcessArgs args, CancellationToken token = new())
    {
        if (CurrentProcess == null)
        {
            Clear();
            if (args.WorkingDirectory != null)
                CurrentDirectory = args.WorkingDirectory;
            var command = args.Filename + ' ' + args.Args;
            Write(command + "\n");
            Box.IsReadOnly = false;
            var proc = await RunProcess(command, args.Stdin, args.Environment, token: token);
            Box.IsReadOnly = true;
            if (proc != null)
                return proc;
        }

        LogService.Logger.Warning("Cannot run new process: another process still running");
        return new CompletedProcess { ExitCode = -1 };
    }

    protected override async Task<ICompletedProcess?> RunProcess(string? command, string? stdin = null,
        EnvironmentModel? environment = null, CancellationToken token = new())
    {
        var proc = await base.RunProcess(command, stdin, environment, token);
        if (proc != null)
        {
            Write($"\nProcess finished with exit code {proc.ExitCode}\n");
        }

        return proc;
    }
}