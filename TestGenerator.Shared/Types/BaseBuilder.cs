using System.Diagnostics;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public abstract class BaseBuilder
{
    public SettingsSection Settings { get; }
    
    public Guid BuildId { get; }
    public AProject Project { get; }
    
    public BaseBuilder(Guid id, AProject project, SettingsSection settings)
    {
        BuildId = id;
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

    public string TempPath => Path.Join(AAppService.Instance.AppDataPath, "Temp", "Builds", BuildId.ToString());
}