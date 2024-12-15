using TestGenerator.Shared.Types;

namespace TestGenerator.Shared.SidePrograms;

public class ProgramOutputValidator : ISideProgramValidator
{
    public Dictionary<string, string> Args { get; } = new()
        { { "Default", "--version" } };

    public Dictionary<string, string> Output { get; } = new()
        { { "Default", "" } };

    public ProgramOutputValidator()
    {
    }

    public ProgramOutputValidator(string args, string output)
    {
        Args["Default"] = args;
        Output["Default"] = output;
    }

    public ProgramOutputValidator(string output)
    {
        Output["Default"] = output;
    }

    public async Task<bool> Validate(SideProgramFile program, CancellationToken token = new())
    {
        string? arg = null;
        string? output = null;
        foreach (var tag in program.VirtualSystem.Tags)
        {
            if (Args.TryGetValue(tag, out arg))
                break;
        }

        foreach (var tag in program.VirtualSystem.Tags)
        {
            if (Output.TryGetValue(tag, out output))
                break;
        }

        if (arg == null || output == null)
            return false;
        try
        {
            var res = await program.Execute(new RunProgramArgs { Args = arg }, token);
            return res.ExitCode == 0 && res.Stdout.Contains(output);
        }
        catch (Exception)
        {
            return false;
        }
    }
}