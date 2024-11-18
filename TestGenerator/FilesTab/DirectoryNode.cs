using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicData;

namespace TestGenerator.FilesTab;

internal class DirectoryNode : Node
{
    private DirectoryInfo Info { get; }

    public DirectoryNode(DirectoryInfo info) : base(info.FullName, info.Name)
    {
        Info = info;
        Initialize();
        Icon =
            "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z";
    }

    private void Initialize()
    {
        foreach (var elem in Info.GetDirectories())
        {
            SubNodes.Add(new DirectoryNode(elem));
        }

        foreach (var elem in Info.GetFiles())
        {
            SubNodes.Add(new FileNode(elem));
        }
    }

    public override bool Exists => Info.Exists;

    public override void Update()
    {

        foreach (var node in SubNodes.ToArray())
        {
            if (!node.Exists)
            {
                SubNodes.Remove(node);
            }
            else
            {
                node.Update();
            }
        }

        SubNodes.AddRange(Info.GetDirectories().Where(d => SubNodes.SingleOrDefault(i => i.Path == d.FullName) == null).Select(d => new DirectoryNode(d)));
        SubNodes.AddRange(Info.GetFiles().Where(d => SubNodes.SingleOrDefault(i => i.Path == d.FullName) == null).Select(d => new FileNode(d)));
    }
}