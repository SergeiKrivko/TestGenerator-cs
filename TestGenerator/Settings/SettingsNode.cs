using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestGenerator.Settings;

internal class SettingsNode
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public ObservableCollection<SettingsNode> Children { get; } = [];
}