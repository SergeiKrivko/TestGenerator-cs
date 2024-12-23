using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Win32.JumpLists;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Services;

public class ProjectsService
{
    private static ProjectsService? _instance;

    private JumpList? _jumpList;

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
        InitJumpList();

        var currentPath = AppService.Instance.Settings.Get<string>("currentProject");
        foreach (var path in AppService.Instance.Settings.Get<string[]>("recentProjects", []))
        {
            var project = Project.Load(path);
            Projects.Add(project);
        }

        if (StartupService.StartupInfo?.LightEdit == true)
        {
            await SetCurrentProject(Project.LightEditProject);
        }
        else if (StartupService.StartupInfo?.Directory != null)
            await SetCurrentProject(Projects.FirstOrDefault(p => p.Path == StartupService.StartupInfo.Directory) ??
                                    Load(StartupService.StartupInfo.Directory));
        else
            await SetCurrentProject(Projects.FirstOrDefault(p => p.Path == currentPath) ?? Project.LightEditProject);

        foreach (var file in StartupService.StartupInfo?.Files ?? [])
        {
            await AppService.Instance.Request<string?>("openFile", file);
        }
    }

    public ObservableCollection<Project> Projects { get; } = new();

    private Project? _current;

    public delegate void ProjectChangeHandler(Project project);

    public event ProjectChangeHandler? CurrentChanged;

    public Project Current => _current ?? Project.LightEditProject;

    public Project Load(string path)
    {
        var proj = Project.Load(path);
        Projects.Insert(0, proj);
        SaveRecentProjects();
        return proj;
    }

    public Project Create(string path, ProjectType type)
    {
        Directory.CreateDirectory(path);
        var proj = Project.Create(path, type);
        Projects.Insert(0, proj);
        SaveRecentProjects();
        return proj;
    }

    public Func<Task<bool>>? TerminateProjectTasksFunc { get; set; }

    public async Task<bool> SetCurrentProject(Project value)
    {
        if (value == Current)
            return false;
        
        if (TerminateProjectTasksFunc != null && !await TerminateProjectTasksFunc())
            return false;
        
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
            Projects.Move(Projects.IndexOf(value), 0);
            SaveRecentProjects();
            AppService.Instance.Settings.Set("currentProject", _current.Path);
        }
        else
        {
            throw new Exception("Unknown project!");
        }

        CurrentChanged?.Invoke(Current);
        AppService.Instance.Emit("projectChanged", Current.Path);
        return true;
    }

    public async Task ReloadProject()
    {
        if (TerminateProjectTasksFunc != null && !await TerminateProjectTasksFunc())
            return;
        LogService.Logger.Debug($"Project '{Current.Name}' reloaded");
        CurrentChanged?.Invoke(Current);
        AppService.Instance.Emit("projectChanged", Current.Path);
        
    }

    private void SaveRecentProjects()
    {
        AppService.Instance.Settings.Set("recentProjects", Projects.Select(p => p.Path));
        if (OperatingSystem.IsWindows() && _jumpList != null)
        {
            _jumpList.JumpItems.RemoveAll(i => (i as JumpTask)?.CustomCategory == "Недавние");
            foreach (var project in Projects.Take(13))
            {
                _jumpList.JumpItems.Add(new JumpTask
                {
                    Title = project.Name,
                    Description = $"Открыть проект '{project.Name}' ({project.Path})",
                    Arguments = $"-d \"{project.Path}\"",
                    CustomCategory = "Недавние",
                });
            }

            JumpList.SetJumpList(Application.Current!, _jumpList);
        }
    }

    private void InitJumpList()
    {
        if (!OperatingSystem.IsWindows())
            return;
        _jumpList = new JumpList
        {
            ShowRecentCategory = false,
            ShowFrequentCategory = false,
        };
        // jumpList1.JumpItemsRejected += JumpList1_JumpItemsRejected;
        // jumpList1.JumpItemsRemovedByUser += JumpList1_JumpItemsRemovedByUser;
        _jumpList.JumpItems.Add(new JumpTask
        {
            Title = "LightEdit",
            Description = "Открыть в режиме LightEdit",
            Arguments = "--light-edit",
            IconResourcePath = @"C:\Windows\notepad.exe",
        });
        JumpList.SetJumpList(Application.Current!, _jumpList);
    }
}