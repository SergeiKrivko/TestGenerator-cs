using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public interface IVirtualSystem
{
    public string Key { get; }
    public string Name { get; }

    public ICollection<string> Tags => ["Default"];

    public Task<ICompletedProcess> Execute(string filename, string args);
    public Task<ICompletedProcess> Execute(string command);

    public bool IsActive => true;

    public Task<string> ConvertPath(string path);
}