using System.Collections.ObjectModel;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Services;

public class BuildsService(BuildTypesService buildTypesService, ProjectsService projectsService)
{
    public ObservableCollection<IBuild> Builds { get; } = [];

    private BuildsService(AppService appService, BuildTypesService buildTypesService, ProjectsService projectsService) :
        this(buildTypesService, projectsService)
    {
        projectsService.CurrentChanged += Load;
        Load(projectsService.Current);
        appService.AddRequestHandler("getAllBuilds", () => Task.FromResult(Builds));
        appService.AddRequestHandler("getBuild", (Guid id) => Task.FromResult(Get(id)));
        appService.AddRequestHandler("createBuild", (string type) => Task.FromResult(New(type)));
    }

    private void Load(IProject project)
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

    public IBuild New(string buildType)
    {
        var build = Build.New(buildTypesService.Get(buildType));
        build.GetBuild = Get;
        Builds.Add(build);
        projectsService.Current.Data.Set("builds", Builds.Select(b => b.Id));
        return build;
    }

    public void Remove(Build build)
    {
        build.Settings.Delete();
        Builds.Remove(build);
        projectsService.Current.Data.Set("builds", Builds.Select(b => b.Id));
    }

    public IBuild? Get(Guid id)
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