using System.Diagnostics;
using Shared.Utils;

namespace Shared;

public abstract class BuildType
{
    public SettingsSection? Settings { get; set; }

    public virtual void Compile()
    {
    }

    public abstract string Command { get; }

    public virtual void Run(string args)
    {
        Process.Start(Command, args);
    }

    public virtual void RunConsole(string args)
    {
    }
}