using System.Collections.ObjectModel;
using System.Reflection;
using Shared;

namespace Backend;

public class ProjectsService
{
    private static ProjectsService? _instance;

    public static ProjectsService Instance
    {
        get
        {
            _instance ??= new ProjectsService();
            return _instance;
        }
    }

    public ObservableCollection<Project> Projects { get; } = new();

    private Project? _current;
    
    public delegate void ProjectChangeHandler(Project project);
    public event ProjectChangeHandler? CurrentChanged;
    
    public Project Current
    {
        get => _current ?? Project.LightEditProject;
        set
        {
            if (value == Project.LightEditProject)
            {
                _current = null;
            }
            else if (Projects.Contains(value))
            {
                _current = value;
            }
            else
            {
                throw new Exception("Unknown project!");
            }
            CurrentChanged?.Invoke(Current);
        }
    }

    public Project Load(string path)
    {
        var proj = Project.Load(path);
        Projects.Add(proj);
        return proj;
    }
}