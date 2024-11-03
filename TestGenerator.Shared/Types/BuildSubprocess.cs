namespace TestGenerator.Shared.Types;

public class BuildSubprocess
{
    public string? Command { get; set; }

    public bool Compile { get; init; } = false;

    public Guid? BuildId { get; set; }
}