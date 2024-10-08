using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace TestGenerator.UI;

public class TerminalTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox); 
    
    public string LastText = "";
    
    public delegate void ReturnHandler(string command);
    
    public event ReturnHandler? Return;
    
    public void Write(string? text)
    {
        if (text == null)
            return;
        var flag = CaretIndex == Text?.Length;
        Text += text;
        if (flag)
            CaretIndex += text.Length;
        LastText = Text;
    }
    
    public string? GetInput()
    {
        return Text?.Replace(LastText, "");
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Delete || e.Key == Key.Back)
        {
            if (SelectionStart <= LastText.Length || CaretIndex <= LastText.Length)
                return;
        }
        else if (e.Key == Key.Return)
        {
            var text = GetInput();
            if (text != null)
            {
                Return?.Invoke(text);
                return;
            }
        }
        else if (Key.A <= e.Key && e.Key <= Key.Z)
        {
            if (Text != null && CaretIndex < LastText.Length)
            {
                CaretIndex = Text.Length;
            }
        }
        base.OnKeyDown(e);
    }
}