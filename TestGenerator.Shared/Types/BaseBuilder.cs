﻿using System.Diagnostics;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public abstract class BaseBuilder
{
    public SettingsSection Settings { get; }

    public Guid BuildId { get; }
    public AProject Project { get; }

    protected BaseBuilder(Guid id, AProject project, SettingsSection settings)
    {
        BuildId = id;
        Settings = settings;
        Project = project;
    }

    public virtual async Task<int> Compile(CancellationToken token = new()) => 0;
    public virtual string? Command => null;

    public virtual async Task<ICompletedProcess> Run(string args = "", string? workingDirectory = null,
        string? stdin = null, CancellationToken token = new())
    {
        return await RunAsync(RunProcessArgs.ProcessRunProvider.Background, args, workingDirectory, stdin, token: token);
    }

    public virtual async Task<ICompletedProcess> RunConsole(string args = "",
        string? workingDirectory = null,
        string? stdin = null,
        CancellationToken token = new())
    {
        return await RunAsync(RunProcessArgs.ProcessRunProvider.RunTab, args, workingDirectory, stdin, token: token);
    }

    private async Task<ICompletedProcess> RunAsync(RunProcessArgs.ProcessRunProvider where, string args,
        string? workingDirectory = null,
        string? stdin = null,
        CancellationToken token = new())
    {
        if (Command == null)
            throw new Exception("Builder base runner: command is null");
        var lst = Command.Split();
        return await AAppService.Instance.RunProcess(where, new RunProcessArgs
        {
            Filename = lst[0],
            Args = string.Join(' ', lst.Skip(1) + args),
            WorkingDirectory = workingDirectory,
            Stdin = stdin
        }, token: token);
    }

    public string TempPath => Path.Join(AAppService.Instance.AppDataPath, "Temp", "Builds", BuildId.ToString());
}