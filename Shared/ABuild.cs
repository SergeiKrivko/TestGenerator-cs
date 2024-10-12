﻿using System.Collections.ObjectModel;

namespace Shared;

public abstract class ABuild
{
    public abstract Guid Id { get; }
    public abstract string Name { get; set; }
    public abstract string WorkingDirectory { get; set; }
    
    public abstract List<BuildSubprocess> PreProc { get; }
    public abstract List<BuildSubprocess> PostProc { get; }
    
    public abstract string Type { get; }

    public abstract BuildType Builder { get; }

    public abstract Task<int> Compile();

    public abstract string? Command { get; }

    public abstract Task<int> Run(string args = "");

    public abstract Task<int> RunConsole(string args = "");
    public abstract Task<int> RunPreProcConsole();
    public abstract Task<int> RunPostProcConsole();
    public abstract Task<int> ExecuteConsole(string args = "");
}