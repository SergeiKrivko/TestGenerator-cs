namespace TestGenerator.Shared.Settings.Shared;

public class EnvironmentModel
{
    public EnvironmentVariable[] Variables { get; init; } = [];
    public bool InheritGlobal { get; init; }
}