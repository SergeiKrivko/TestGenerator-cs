using System.Collections.ObjectModel;
using Core.Types;

namespace Core.Services;

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
                LogService.Logger.Debug("Current project set to LightEdit");
                _current = null;
            }
            else if (Projects.Contains(value))
            {
                LogService.Logger.Debug($"Current project set to '{value.Name}'");
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