namespace Shared;

public abstract class ABuild
{
    public abstract Guid Id { get; }
    public abstract string Name { get; set; }
    public abstract string WorkingDirectory { get; set; }
    
    public abstract string Type { get; }

    public abstract BuildType Builder { get; }

    public abstract void Compile();

    public abstract string Command { get; }

    public abstract void Run(string args);

    public abstract void RunConsole(string args);
}