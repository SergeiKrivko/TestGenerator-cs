﻿using System.Text.RegularExpressions;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;

namespace TestGenerator.Shared;

public abstract class Plugin
{
    public required string Name { get; init; }
    public List<MainTab> MainTabs { get; init; } = [];
    [Obsolete("Use 'SideItems' instead")] public List<SideTab> SideTabs { get; init; } = [];
    public List<ISideItem> SideItems { get; init; } = [];
    public List<BuildType> BuildTypes { get; init; } = [];
    public List<ProjectType> ProjectTypes { get; init; } = [];
    public List<IEditorProvider> EditorProviders { get; init; } = [];
    public List<IFileCreator> FileCreators { get; init; } = [];
    public List<IFileAction> FileActions { get; init; } = [];
    public List<SettingsNode> SettingsControls { get; init; } = [];
    public Dictionary<string, string> FileIcons { get; init; } = [];
    public Dictionary<Regex, string> RegexFileIcons { get; init; } = [];

    protected Plugin()
    {
    }

    protected Plugin(
        string name,
        List<MainTab>? mainTabs = null,
        List<ISideItem>? sideItems = null,
        List<BuildType>? buildTypes = null,
        List<ProjectType>? projectTypes = null,
        List<IEditorProvider>? editorProviders = null,
        List<IFileCreator>? fileCreators = null,
        List<IFileAction>? fileActions = null,
        List<SettingsNode>? settingsControls = null,
        Dictionary<string, string>? fileIcons = null,
        Dictionary<Regex, string>? regexFileIcons = null)
    {
        Name = name;
        if (mainTabs != null)
            MainTabs = mainTabs;
        if (sideItems != null)
            SideItems = sideItems;
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
        if (regexFileIcons != null)
            RegexFileIcons = regexFileIcons;
    }

    public virtual async Task Init(CancellationToken token)
    {
    }

    public virtual async Task Destroy()
    {
    }
}