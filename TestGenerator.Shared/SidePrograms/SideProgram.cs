namespace TestGenerator.Shared.SidePrograms;

public class SideProgram
{
    public required string Key { get; init; }
    public List<ISideProgramValidator> Validators { get; init; } = [];

    public Dictionary<string, ICollection<string>> Locations { get; init; } = [];

    public SideProgramFile FromPath(string path)
    {
        return new SideProgramFile(this, path, NoVirtualSystem);
    }

    public SideProgramFile FromPath(string path, IVirtualSystem virtualSystem)
    {
        return new SideProgramFile(this, path, virtualSystem);
    }

    public SideProgramFile FromPath(string path, string virtualSystem)
    {
        return new SideProgramFile(this, path, VirtualSystems.Single(s => s.Key == virtualSystem));
    }

    public SideProgramFile? FromModel(ProgramFileModel? model)
    {
        Console.WriteLine(model?.Path);
        if (model?.Program != Key)
            return null;
        var virtualSystem = VirtualSystems.SingleOrDefault(s => s.Key == model.VirtualSystem);
        return virtualSystem == null ? null : new SideProgramFile(this, model.Path, virtualSystem);
    }

    public async Task<ICollection<SideProgramFile>> Search()
    {
        var res = new List<SideProgramFile>();
        foreach (var virtualSystem in VirtualSystems.Where(s => s.IsActive))
        {
            foreach (var tag in virtualSystem.Tags)
            {
                foreach (var location in Locations.GetValueOrDefault(tag, []))
                {
                    var prog = FromPath(location, virtualSystem);
                    if (await prog.Validate())
                        res.Add(prog);
                }
            }
        }

        return res;
    }
    
    private static readonly IVirtualSystem NoVirtualSystem = new NoVirtualSystem();

    public static List<IVirtualSystem> VirtualSystems { get; } = [NoVirtualSystem];

    public static bool HasVirtualSystem(string key)
    {
        return VirtualSystems.FirstOrDefault(s => s.Key == key) == null;
    }
}