namespace TestGenerator.Shared.Types;

public class ProjectType
{
    public string Key { get; }
    public string Name { get; }
    public string IconPath { get; }

    public delegate bool ProjectTypeDetectorFunc(string path);

    public class ProjectTypeDetector
    {
        public int Priority { get; }
        public ProjectTypeDetectorFunc Func { get; }

        public ProjectTypeDetector(int priority, ProjectTypeDetectorFunc func)
        {
            if (priority < 0 || priority > 10)
                throw new Exception("Priority must be between 0 and 10");
            Priority = priority;
            Func = func;
        }
    }

    public List<ProjectTypeDetector> Detectors { get; }
    public List<IProjectCreator> Creators { get; }

    public ProjectType(string key, string name, string iconPath, List<ProjectTypeDetector>? detectors = null)
    {
        Key = key;
        Name = name;
        IconPath = iconPath;
        Detectors = detectors ?? [];
        Creators = [];
    }

    public ProjectType(string key, string name, string iconPath, List<ProjectTypeDetector>? detectors = null,
        List<IProjectCreator>? creators = null)
    {
        Key = key;
        Name = name;
        IconPath = iconPath;
        Detectors = detectors ?? [];
        Creators = creators ?? [];
    }
}