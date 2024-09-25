using System;
using System.IO;
using Avalonia.Controls;
using AvaloniaEdit.Document;

namespace TestGenerator.MainTabs.Code;

public partial class CodeEditor : UserControl
{
    public CodeEditor()
    {
        InitializeComponent();
    }

    public void Open(string fileName)
    {
        using StreamReader reader = new(fileName);
        Editor.Text = reader.ReadToEnd();
    }
}