using System.Collections.ObjectModel;
using TestGenerator.Core.Types;

namespace TestGenerator.Core.Services;

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

    private ProjectsService()
    {
    }

    public async void Load()
    {
        var currentPath = StartupService.StartupInfo?.Directory ??
                          AppService.Instance.Settings.Get<string>("currentProject");
        foreach (var path in AppService.Instance.Settings.Get<string[]>("recentProjects", []))
        {
            var project = Project.Load(path);
            Projects.Add(project);
            if (path == currentPath)
                Current = project;
        }

        if (Current == Project.LightEditProject && StartupService.StartupInfo?.Directory != null)
        {
            Current = Load(StartupService.StartupInfo.Directory);
        }

        foreach (var file in StartupService.StartupInfo?.Files ?? [])
        {
            await AppService.Instance.Request<string?>("openFile", file);
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
            if (value == Current)
                return;
            if (value == Project.LightEditProject)
            {
                LogService.Logger.Debug("Current project set to LightEdit");
                _current = null;
                AppService.Instance.Settings.Remove("currentProject");
            }
            else if (Projects.Contains(value))
            {
                LogService.Logger.Debug($"Current project set to '{value.Name}'");
                _current = value;
                AppService.Instance.Settings.Set("currentProject", _current.Path);
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
        AppService.Instance.Settings.Set("recentProjects", Projects.Select(p => p.Path));
        return proj;
    }
}