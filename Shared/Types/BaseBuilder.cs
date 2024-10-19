using System.Diagnostics;
using Shared.Utils;

namespace Shared.Types;

public abstract class BaseBuilder
{
    public SettingsSection Settings { get; }
    public AProject Project { get; }
    
    public BaseBuilder(AProject project, SettingsSection settings)
    {
        Settings = settings;
        Project = project;
    }

    public virtual async Task<int> Compile() => 0;
    public virtual string? Command => null;
    
    public virtual async Task<int> Run(string args = "")
    {
        if (Command == null)
            throw new Exception("Builder base runner: command is null");
        var lst = Command.Split();
        var proc = Process.Start(lst[0], string.Join(' ', lst.Skip(1)) + " " + args);
        await proc.WaitForExitAsync();
        return proc.ExitCode;
    }

    public virtual async Task<int> RunConsole(string args)
    {
        if (Command == null)
            throw new Exception("Builder base runner: command is null");
        var controller = AAppService.Instance.RunInConsole(Command + " " + args, 
            Settings.Get<string>("workingDirectory"));
        return await controller.RunAsync();
    }
}