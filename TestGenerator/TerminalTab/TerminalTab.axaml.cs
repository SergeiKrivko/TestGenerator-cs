using Shared;

namespace TestGenerator.TerminalTab;

public partial class TerminalTab : SideTab
{
    public override string TabKey => "Terminal";
    public override string TabName => "Терминал";
    public override string TabIcon => "M3.05178e-05 4C3.05178e-05 4 3.05176e-05 7.53815e-06 4.00003 2.31943e-06H20C20 2.31943e-06 24 2.38419e-06 24 4V14C24 14 24 18 20 18H4.00003C4.00003 18 3.05178e-05 18 3.05178e-05 14V4ZM4.00003 1C4.00003 1 1 1 1.00003 4V14C1 17 4.00003 17 4.00003 17H20C20 17 23 17 23 14.5V4C23 1 20 1 20 1H4.00003ZM9 10.5C10.4 9 9 7.5 9 7.5L5 3L3.5 4.5L7.5 9L3.5 13.5L5 15L9 10.5ZM10 15H20V13H10V15Z";

    public TerminalTab()
    {
        InitializeComponent();
    }
}