using Core.Types;
using Shared;

namespace Core.Services;

public class BuildTypesService
{
    private static BuildTypesService? _instance;

    public static BuildTypesService Instance
    {
        get
        {
            _instance ??= new BuildTypesService();
            return _instance;
        }
    }

    public Dictionary<string, Type> Types { get; } = new();

    public Type? Get(string key)
    {
        return Types.GetValueOrDefault(key, typeof(EmptyBuild));
    }

    public void Add(string key, Type buildType)
    {
        Types.Add(key, buildType);
    }
}