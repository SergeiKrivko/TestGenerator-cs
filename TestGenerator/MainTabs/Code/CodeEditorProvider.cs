using System.Collections.Generic;
using System.IO;
using TestGenerator.FilesTab;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public class CodeEditorProvider : IEditorProvider
{
    public string Key => "CodeEditor";
    public string Name => "Редактор кода";
    public string[]? Extensions => null;
    public int Priority => 5;

    public OpenedFile Open(string path)
    {
        var widget = new CodeEditor();
        widget.Open(path);
        return new OpenedFile
        {
            Name = Path.GetFileName(path), Widget = widget, Path = path,
            Icon = FileNode.FileIcons.GetValueOrDefault(Path.GetExtension(path),
                "M12 3C7.031 3 3 7.031 3 12C3 16.969 7.031 21 12 21C16.969 21 21 16.969 21 12C21 7.031 16.969 3 12 3ZM11.719 17.25C11.5933 17.2541 11.4681 17.2329 11.3507 17.1876C11.2334 17.1424 11.1264 17.074 11.036 16.9865C10.9457 16.899 10.8738 16.7943 10.8247 16.6785C10.7757 16.5627 10.7504 16.4383 10.7504 16.3125C10.7504 16.1867 10.7757 16.0623 10.8247 15.9465C10.8738 15.8307 10.9457 15.726 11.036 15.6385C11.1264 15.551 11.2334 15.4826 11.3507 15.4374C11.4681 15.3921 11.5933 15.3709 11.719 15.375C11.9623 15.3829 12.1931 15.4852 12.3624 15.6602C12.5317 15.8351 12.6264 16.069 12.6264 16.3125C12.6264 16.556 12.5317 16.7899 12.3624 16.9648C12.1931 17.1398 11.9623 17.2421 11.719 17.25ZM13.286 12.469C12.526 12.979 12.422 13.446 12.422 13.875C12.422 14.0491 12.3528 14.2161 12.2297 14.3392C12.1066 14.4623 11.9396 14.5315 11.7655 14.5315C11.5914 14.5315 11.4244 14.4623 11.3013 14.3392C11.1782 14.2161 11.109 14.0491 11.109 13.875C11.109 12.848 11.582 12.031 12.554 11.378C13.458 10.772 13.969 10.388 13.969 9.542C13.969 8.968 13.641 8.532 12.961 8.208C12.801 8.132 12.446 8.058 12.008 8.063C11.458 8.07 11.032 8.202 10.703 8.466C10.083 8.965 10.031 9.508 10.031 9.516C10.0268 9.60215 10.0057 9.68662 9.96882 9.7646C9.93197 9.84259 9.88012 9.91255 9.81623 9.97049C9.75235 10.0284 9.67767 10.0732 9.59648 10.1023C9.51528 10.1314 9.42915 10.1442 9.343 10.14C9.25685 10.1358 9.17238 10.1147 9.0944 10.0778C9.01641 10.041 8.94645 9.98912 8.88851 9.92523C8.83057 9.86135 8.78577 9.78667 8.75669 9.70548C8.7276 9.62428 8.7148 9.53815 8.719 9.452C8.724 9.338 8.803 8.312 9.879 7.446C10.439 6.997 11.149 6.764 11.989 6.753C12.584 6.746 13.144 6.847 13.523 7.026C14.658 7.563 15.281 8.458 15.281 9.542C15.281 11.128 14.221 11.84 13.286 12.469Z")
        };
    }

    public bool CanOpen(string path) => !IsBinary(path);
    
    private static bool IsBinary(string filePath, int requiredConsecutiveNul = 1)
    {
        const int charsToCheck = 8000;
        const char nulChar = '\0';

        int nulCount = 0;

        using (var streamReader = new StreamReader(filePath))
        {
            for (var i = 0; i < charsToCheck; i++)
            {
                if (streamReader.EndOfStream)
                    return false;

                if ((char) streamReader.Read() == nulChar)
                {
                    nulCount++;

                    if (nulCount >= requiredConsecutiveNul)
                        return true;
                }
                else
                {
                    nulCount = 0;
                }
            }
        }

        return false;
    }
}