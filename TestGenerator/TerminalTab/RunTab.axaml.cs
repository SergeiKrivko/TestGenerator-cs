using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Shared;

namespace TestGenerator.TerminalTab;

public partial class RunTab : SideTab
{
    public override string TabKey => "Run";
    public override string TabName => "Выполнение";
    public override string TabIcon => "M6.23405 20.625C5.94705 20.625 5.66405 20.549 5.41405 20.406C5.13374 20.242 4.90176 20.0067 4.7416 19.7242C4.58145 19.4416 4.49881 19.1218 4.50204 18.797V5.20301C4.50204 4.53001 4.85205 3.91301 5.41405 3.59401C5.66983 3.44699 5.96041 3.37137 6.25542 3.37507C6.55042 3.37876 6.83902 3.46163 7.09105 3.61501L18.709 10.569C18.9517 10.7205 19.1517 10.9313 19.2904 11.1815C19.4291 11.4316 19.5019 11.713 19.5019 11.999C19.5019 12.2851 19.4291 12.5664 19.2904 12.8166C19.1517 13.0667 18.9517 13.2775 18.709 13.429L7.08905 20.385C6.83105 20.541 6.53552 20.6239 6.23405 20.625Z";

    public RunTab()
    {
        InitializeComponent();
    }

    public override void Command(string command, string? data)
    {
        switch (command)
        {
            case "run":
                Terminal.Run(data);
                break;
            case "changeDirectory":
                if (!string.IsNullOrEmpty(data))
                    Terminal.CurrentDirectory = data;
                break;
        }
    }
}