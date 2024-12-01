using System.Collections.ObjectModel;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Services;

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

    public ObservableCollection<ABuild> Builds { get; } = [];

    private BuildsService()
    {
        ProjectsService.Instance.CurrentChanged += Load;
        Load(ProjectsService.Instance.Current);
        AppService.Instance.AddRequestHandler("getAllBuilds", async () => Builds);
        AppService.Instance.AddRequestHandler("getBuild", async (Guid id) => Get(id));
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

    public ABuild New(string buildType)
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
        ProjectsService.Instance.Current.Data.Set("builds", Builds.Select(b => b.Id));
    }

    public ABuild? Get(Guid id)
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