using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Shared.Types;

public interface IBuild
{
    public Guid Id { get; }
    public string Name { get; set; }
    public string WorkingDirectory { get; set; }
    public EnvironmentModel? Environment { get; set; }

    public List<BuildSubprocess> PreProc { get; }
    public List<BuildSubprocess> PostProc { get; }

    public string TypeName { get; }

    public BuildType Type { get; }
    public BaseBuilder Builder { get; }

    public Task<int> Compile(CancellationToken token = new());

    public string? Command { get; }

    public Task<ICompletedProcess> Run(string args = "", string? stdin = null, CancellationToken token = new());

    public Task<ICompletedProcess> RunConsole(string args = "", string? stdin = null, CancellationToken token = new());
    public Task<int> RunPreProc(CancellationToken token = new());
    public Task<int> RunPostProc(CancellationToken token = new());
    public Task<int> RunPreProcConsole(CancellationToken token = new());
    public Task<int> RunPostProcConsole(CancellationToken token = new());
    public Task<int> Execute(string args = "", string? stdin = null, CancellationToken token = new());
    public Task<int> ExecuteConsole(string args = "", string? stdin = null, CancellationToken token = new());
}