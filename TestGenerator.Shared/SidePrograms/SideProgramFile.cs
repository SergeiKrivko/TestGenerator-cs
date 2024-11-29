using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public class SideProgramFile
{
    public SideProgram Program { get; }
    public string Path { get; }
    public bool IsValid { get; private set; } = true;
    public IVirtualSystem VirtualSystem { get; }

    private static readonly IVirtualSystem NoVirtualSystem = new NoVirtualSystem();

    internal SideProgramFile(SideProgram program, string path, IVirtualSystem? virtualSystem = null)
    {
        Program = program;
        Path = path;
        VirtualSystem = virtualSystem ?? NoVirtualSystem;
        ValidateAsync();
    }

    private async void ValidateAsync() => await Validate();

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
}