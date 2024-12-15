namespace TestGenerator.Shared.SidePrograms;

public interface ISideProgramValidator
{
    public Task<bool> Validate(SideProgramFile program, CancellationToken token = new());
}