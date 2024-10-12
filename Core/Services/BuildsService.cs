using System.Collections.ObjectModel;
using Core.Types;

namespace Core.Services;

public class BuildsService
{
    private static BuildsService? _instance;

    public static BuildsService Instance
    {
        get
        {
            _instance ??= new BuildsService();
            return _instance;
        }
    }

    public ObservableCollection<Build> Builds { get; } = [];

    private BuildsService()
    {
        ProjectsService.Instance.CurrentChanged += Load;
        Load(ProjectsService.Instance.Current);
    }

    private void Load(Project project)
    {
        Builds.Clear();
        LogService.Logger.Debug($"Loading {project.Data.Get<Guid[]>("builds", []).Length} builds");
        foreach (var buildId in project.Data.Get<Guid[]>("builds", []))
        {
            var build = Build.Load(buildId);
            build.GetBuild = Get;
            Builds.Add(build);
        }
    }

    public Build New(string buildType)
    {
        var build = Build.New(BuildTypesService.Instance.Get(buildType));
        build.GetBuild = Get;
        Builds.Add(build);
        ProjectsService.Instance.Current.Data.Set("builds", Builds.Select(b => b.Id));
        return build;
    }

    public void Remove(Build build)
    {
        build.Settings.Delete();
        Builds.Remove(build);
    }

    public Build? Get(Guid id)
    {
        try
        {
            return Builds.Single(b => b.Id == id);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}