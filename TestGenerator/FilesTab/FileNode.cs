using System.IO;

namespace TestGenerator.FilesTab;

internal class FileNode : Node
{
    public FileInfo Info { get; }

    public FileNode(FileInfo info) : base(info.FullName, info.Name)
    {
        Info = info;
    }
}