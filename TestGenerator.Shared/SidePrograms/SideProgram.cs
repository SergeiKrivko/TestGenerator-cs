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
        if (model?.Program != Key)
            return null;
        var virtualSystem = VirtualSystems.SingleOrDefault(s => s.Key == model.VirtualSystem);
        return virtualSystem == null ? null : new SideProgramFile(this, model.Path, virtualSystem);
    }

    private bool _searchStarted = false;
    private ICollection<SideProgramFile>? _programFiles;
    private int _vsHash = 0;

    public async Task<ICollection<SideProgramFile>> Search(CancellationToken token = new())
    {
        if (_searchStarted)
        {
            while (_programFiles == null)
            {
                await Task.Delay(100, token);
            }

            return _programFiles;
        }
        var virtualSystems = VirtualSystems.Where(s => s.IsActive).ToArray();
        if (virtualSystems.GetHashCode() != _vsHash && _programFiles != null)
            return _programFiles;
        _vsHash = virtualSystems.GetHashCode();
        _searchStarted = true;
        var res = new List<SideProgramFile>();
        foreach (var virtualSystem in virtualSystems)
        {
            foreach (var tag in virtualSystem.Tags)
            {
                foreach (var location in Locations.GetValueOrDefault(tag, []))
                {
                    var prog = FromPath(Environment.ExpandEnvironmentVariables(location), virtualSystem);
                    if (res.FirstOrDefault(p => p.Path == prog.Path && p.VirtualSystem.Key == prog.VirtualSystem.Key) ==
                        null && await prog.Validate(token: token))
                        res.Add(prog);
                }
            }
        }

        _programFiles = res;
        return res;
    }

    private static readonly IVirtualSystem NoVirtualSystem = new NoVirtualSystem();

    public static List<IVirtualSystem> VirtualSystems { get; } = [NoVirtualSystem];

    public static bool HasVirtualSystem(string key)
    {
        return VirtualSystems.FirstOrDefault(s => s.Key == key) == null;
    }
}