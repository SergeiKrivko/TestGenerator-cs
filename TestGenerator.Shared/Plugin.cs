using Avalonia.Controls;
using TestGenerator.Shared.Types;

namespace TestGenerator.Shared;

public class Plugin
{
    public required string Name { get; init; }
    public List<MainTab> MainTabs { get; init; } = [];
    public List<SideTab> SideTabs { get; init; } = [];
    public List<BuildType> BuildTypes { get; init; } = [];
    public List<ProjectType> ProjectTypes { get; init; } = [];
    public List<IEditorProvider> EditorProviders { get; init; } = [];
    public List<IFileCreator> FileCreators { get; init; } = [];
    public List<IFileAction> FileActions { get; init; } = [];
    public Dictionary<string, Control> SettingsControls { get; init; } = [];
    public Dictionary<string, IconElement> FileIcons { get; init; } = [];

    protected Plugin()
    {
    }

    protected Plugin(
        string name,
        List<MainTab>? mainTabs = null,
        List<SideTab>? sideTabs = null,
        List<BuildType>? buildTypes = null,
        List<ProjectType>? projectTypes = null,
        List<IEditorProvider>? editorProviders = null,
        List<IFileCreator>? fileCreators = null,
        List<IFileAction>? fileActions = null,
        Dictionary<string, Control>? settingsControls = null,
        Dictionary<string, IconElement>? fileIcons = null)
    {
        Name = name;
        if (mainTabs != null)
            MainTabs = mainTabs;
        if (sideTabs != null)
            SideTabs = sideTabs;
        if (buildTypes != null)
            BuildTypes = buildTypes;
        if (projectTypes != null)
            ProjectTypes = projectTypes;
        if (editorProviders != null)
            EditorProviders = editorProviders;
        if (fileCreators != null)
            FileCreators = fileCreators;
        if (fileActions != null)
            FileActions = fileActions;
        if (settingsControls != null)
            SettingsControls = settingsControls;
        if (fileIcons != null)
            FileIcons = fileIcons;
    }
}