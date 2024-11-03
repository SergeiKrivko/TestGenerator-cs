using System;
using Avalonia.Controls;
using Avalonia.Media;
using TestGenerator.Shared.Types;

namespace TestGenerator.Builds;

public class BuildTypeItem : MenuItem
{
    protected override Type StyleKeyOverride => typeof(MenuItem); 
    
    public BuildTypeItem(BuildType buildType)
    {
        Header = buildType.Name;
        Icon = new PathIcon { Data = PathGeometry.Parse(buildType.Icon) };
        Click += (sender, args) => Selected?.Invoke(buildType.Key);
    }

    public delegate void ClickHandler(string type);
    
    public event ClickHandler? Selected;
}