using System.Threading.Tasks;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public partial class CodeTab : MainTab
{
    public CodeTab()
    {
        InitializeComponent();
        AAppService.Instance.AddRequestHandler<string, int>("openFile", Open);
    }

    private async Task<int> Open(string path)
    {
        Editor.Open(path);
        return 0;
    }
}