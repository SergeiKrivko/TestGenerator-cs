﻿using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public abstract class SideTab : UserControl
{
    public virtual string TabKey { get; } = new Guid().ToString();
    public abstract string TabName { get; }
    public abstract string TabIcon { get; }

    public virtual int TabPriority { get; } = 10;
}