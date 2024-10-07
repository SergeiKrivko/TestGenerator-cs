using Shared;

namespace Backend.Services;

public class ProjectTypesService
{
    private static ProjectTypesService? _instance;

    public static ProjectTypesService Instance
    {
        get
        {
            _instance ??= new ProjectTypesService();
            return _instance;
        }
    }

    public static ProjectType Default { get; } = new("", "Unknown",
        "M0.25 4.5C0.25 3.5 1.75 0 5.25 0C8.75 0 10.75 2 10.75 4.5C10.75 7 9.25 8 9.25 8L6.75 10C6.75 10 5.75 " +
        "11 5.75 12C5.75 12.5 5.375 12.75 5 12.75V13.5C5 13.5 6.5 13.5 6.5 15C6.5 16.5 5 16.5 5 16.5C5 16.5 3.5 16.5 " +
        "3.5 15C3.5 13.5 5 13.5 5 13.5V12.75C4.625 12.75 4.25 12.5 4.25 12C4.25 11 4.75 9.5 6.25 8.5C7.75 7.5 9.25 " +
        "6.66431 9.25 4.5C9.25 2.33569 7.25 1.5 5.25 1.5C3.25 1.5 1.75 3.5 1.75 4.5C1.75 5.5 0.25 5.5 0.25 4.5Z");

    public Dictionary<string, ProjectType> Types { get; } = new();

    public ProjectType Get(string key)
    {
        return Types.GetValueOrDefault(key, Default);
    }
}