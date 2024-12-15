using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public class SideProgramFile
{
    public SideProgram Program { get; }
    public string Path { get; }
    public bool IsValid { get; private set; } = true;
    public DateTime? LastValidation { get; private set; }
    public IVirtualSystem VirtualSystem { get; }

    internal SideProgramFile(SideProgram program, string path, IVirtualSystem virtualSystem)
    {
        Program = program;
        Path = path;
        VirtualSystem = virtualSystem;
    }

    public async void ValidateAsync() => await Validate();

    public async Task<bool> Validate(bool force = false, CancellationToken token = new())
    {
        if (!force && LastValidation != null)
            return IsValid;
        foreach (var validator in Program.Validators)
        {
            if (!await validator.Validate(this, token))
            {
                IsValid = false;
                LastValidation = DateTime.Now;
                return false;
            }
        }

        IsValid = true;
        LastValidation = DateTime.Now;
        return IsValid;
    }

    public async Task<ICompletedProcess> Execute(RunProcessArgs.ProcessRunProvider where, RunProgramArgs args, CancellationToken token = new())
    {
        return await VirtualSystem.Execute(where, args.ToProcessArgs(Path), token);
    }

    public async Task<ICompletedProcess> Execute(RunProgramArgs args, CancellationToken token = new())
    {
        return await VirtualSystem.Execute(args.ToProcessArgs(Path), token);
    }

    public async Task<ICollection<ICompletedProcess>> Execute(RunProcessArgs.ProcessRunProvider where, RunProgramArgs[] args, CancellationToken token = new())
    {
        return await VirtualSystem.Execute(where, args.Select(a => a.ToProcessArgs(Path)).ToArray(), token);
    }

    public async Task<ICollection<ICompletedProcess>> Execute(RunProgramArgs[] args, CancellationToken token = new())
    {
        return await VirtualSystem.Execute(args.Select(a => a.ToProcessArgs(Path)).ToArray(), token);
    }

    public ProgramFileModel ToModel()
    {
        return new ProgramFileModel { Path = Path, Program = Program.Key, VirtualSystem = VirtualSystem.Key };
    }

    internal string DisplayName => VirtualSystem.Key == "" ? Path : $"[{VirtualSystem.Name}] {Path}";
}