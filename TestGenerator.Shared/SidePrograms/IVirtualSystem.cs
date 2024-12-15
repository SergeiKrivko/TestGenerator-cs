using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public interface IVirtualSystem
{
    public string Key { get; }
    public string Name { get; }

    public ICollection<string> Tags => ["Default"];

    public Task<ICompletedProcess> Execute(RunProcessArgs.ProcessRunProvider where, RunProcessArgs args, CancellationToken token = new());
    public Task<ICompletedProcess> Execute(RunProcessArgs args, CancellationToken token = new());
    public Task<ICollection<ICompletedProcess>> Execute(RunProcessArgs.ProcessRunProvider where, RunProcessArgs[] args, CancellationToken token = new());
    public Task<ICollection<ICompletedProcess>> Execute(RunProcessArgs[] args, CancellationToken token = new());

    public bool IsActive => true;

    public Task<string> ConvertPath(string path);
}