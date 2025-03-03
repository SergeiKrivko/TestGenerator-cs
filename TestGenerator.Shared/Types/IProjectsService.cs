namespace TestGenerator.Shared.Types;

public interface IProjectsService
{
    public event Action<IProject> CurrentChanged;
    public IProject Current { get; }
}