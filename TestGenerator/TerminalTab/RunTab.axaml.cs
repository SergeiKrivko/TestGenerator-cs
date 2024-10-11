using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Shared;

namespace TestGenerator.TerminalTab;

public partial class RunTab : SideTab
{
    public RunTab()
    {
        TabKey = "Run";
        TabName = "Запуск";
        InitializeComponent();
    }

    public override void Command(string command, string? data)
    {
        switch (command)
        {
            case "run":
                Terminal.Run(data);
                break;
        }
    }
}