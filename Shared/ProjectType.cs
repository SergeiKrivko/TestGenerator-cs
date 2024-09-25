namespace Shared;

public class ProjectType
{
    public string Key { get; }
    public string Name { get; }
    public string IconPath { get; }

    public ProjectType(string key, string name, string iconPath)
    {
        Key = key;
        Name = name;
        IconPath = iconPath;
    }
}