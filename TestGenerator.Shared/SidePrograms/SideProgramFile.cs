using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public class SideProgramFile
{
    public SideProgram Program { get; }
    public string Path { get; }
    public bool IsValid { get; private set; } = true;
    public IVirtualSystem VirtualSystem { get; }

    internal SideProgramFile(SideProgram program, string path, IVirtualSystem virtualSystem)
    {
        Program = program;
        Path = path;
        VirtualSystem = virtualSystem;
    }

    public async void ValidateAsync() => await Validate();

    public async Task<bool> Validate()
    {
        foreach (var validator in Program.Validators)
        {
            if (!await validator.Validate(this))
            {
                IsValid = false;
                return false;
            }
        }

        IsValid = true;
        return IsValid;
    }

    public async Task<ICompletedProcess> Execute(string command)
    {
        return await VirtualSystem.Execute(Path, command);
    }

    public ITerminalController ExecuteInConsole(string command, string? workingDirectory = null)
    {
        return VirtualSystem.ExecuteInConsole($"{Path} {command}", workingDirectory);
    }

    public ITerminalController ExecuteInConsole(string command)
    {
        return VirtualSystem.ExecuteInConsole($"{Path} {command}");
    }

    public ProgramFileModel ToModel()
    {
        return new ProgramFileModel { Path = Path, Program = Program.Key, VirtualSystem = VirtualSystem.Key };
    }
}