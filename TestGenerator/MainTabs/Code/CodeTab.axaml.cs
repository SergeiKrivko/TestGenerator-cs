using Shared;

namespace TestGenerator.MainTabs.Code;

public partial class CodeTab : MainTab
{
    public CodeTab()
    {
        InitializeComponent();
    }

    public override void Command(string command, string? data)
    {
        switch (command)
        {
            case "open":
                if (data is not null)
                    Editor.Open(data);
                break;
        }
    }
}