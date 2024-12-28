using TestGenerator.Shared.Settings.Shared;
using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public class RunProgramArgs
{
    public string Args { get; init; } = "";
    public string? WorkingDirectory { get; init; }
    public string? Stdin { get; init; }
    public EnvironmentModel? Environment { get; init; }

    internal RunProcessArgs ToProcessArgs(string filename)
    {
        return new RunProcessArgs
        {
            Filename = filename, Args = Args, WorkingDirectory = WorkingDirectory, Stdin = Stdin,
            Environment = Environment
        };
    }
}