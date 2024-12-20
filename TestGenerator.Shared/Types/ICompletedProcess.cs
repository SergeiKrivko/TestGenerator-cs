﻿namespace TestGenerator.Shared.Types;

public interface ICompletedProcess
{
    public int ExitCode { get; }
    public string Stdout { get; }
    public string Stderr { get; } 
    public TimeSpan Time { get; }
}