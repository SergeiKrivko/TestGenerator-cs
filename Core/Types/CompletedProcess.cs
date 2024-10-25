using Shared.Types;

namespace Core.Types;

public class CompletedProcess : ICompletedProcess
{
    public required int ExitCode { get; init; }
    public string Stdout { get; init; } = "";
    public string Stderr { get; init; } = "";
}