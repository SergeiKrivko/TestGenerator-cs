using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace TestGenerator.MainTabs.Code;

public partial class CodeEditor : UserControl
{
    private readonly TextMate.Installation _textMateInstallation;
    private readonly RegistryOptions _registryOptions;

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

    public void Open(string fileName)
    {
        _textMateInstallation.SetGrammar(null);
        Editor.Document = new TextDocument(File.ReadAllText(fileName));
        try
        {
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(
                _registryOptions.GetLanguageByExtension("." + fileName.Split('.')[^1]).Id
            ));
        }
        catch (NullReferenceException)
        {
        }
    }
}