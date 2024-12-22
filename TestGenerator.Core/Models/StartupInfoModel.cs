namespace TestGenerator.Core.Models;

public class StartupInfoModel
{
    public bool StartGui { get; init; } = true;
    public string? Directory { get; init; } = null;
    public ICollection<string> Files { get; init; } = [];
    public bool LightEdit { get; init; }
}