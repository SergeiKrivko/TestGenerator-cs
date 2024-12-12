using System;
using System.IO;
using Avalonia.Controls;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace TestGenerator.MainTabs.Code;

public partial class CodeEditor : UserControl
{
    private readonly TextMate.Installation _textMateInstallation;
    private readonly RegistryOptions _registryOptions;

    public string? FileName { get; private set; }

    public CodeEditor()
    {
        InitializeComponent();

        _registryOptions = new RegistryOptions(ThemeName.DarkPlus);

        //Initial setup of TextMate.
        _textMateInstallation = Editor.InstallTextMate(_registryOptions);

        //Here we are getting the language by the extension and right after that we are initializing grammar with this language.
        //And that's all 😀, you are ready to use AvaloniaEdit with syntax highlighting!
        _textMateInstallation.SetGrammar(
            _registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
    }

    public async void Open(string fileName)
    {
        FileName = fileName;

        _textMateInstallation.SetGrammar(null);
        Editor.Document = new TextDocument(await File.ReadAllTextAsync(fileName));
        try
        {
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(
                string.Equals(Path.GetFileNameWithoutExtension(fileName), "makefile",
                    StringComparison.InvariantCultureIgnoreCase)
                    ? "makefile"
                    : _registryOptions.GetLanguageByExtension(Path.GetExtension(fileName)).Id
            ));
        }
        catch (NullReferenceException)
        {
        }
    }

    public async void Save()
    {
        if (FileName == null)
            return;
        await File.WriteAllTextAsync(FileName, Editor.Text);
    }

    private void Editor_OnTextChanged(object? sender, EventArgs e)
    {
        Save();
    }
}