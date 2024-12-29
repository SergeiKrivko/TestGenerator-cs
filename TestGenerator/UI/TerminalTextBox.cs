using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace TestGenerator.UI;

public class TerminalTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox); 
    
    private string LastText { get; set; } = "";
    
    public delegate void ReturnHandler(string command);
    
    public event ReturnHandler? Return;
    
    public delegate void ArrowHandler();
    
    public event ArrowHandler? ArrowUp;
    public event ArrowHandler? ArrowDown;
    
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
    
    private string? GetInput()
    {
        return Text?.Replace(LastText, "");
    }

    public void ClearText()
    {
        LastText = "";
        Text = "";
    }

    public void Rewrite(string text = "")
    {
        Text = LastText + text;
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsReadOnly)
            return;
        if ((e.KeyModifiers & (KeyModifiers.Shift | KeyModifiers.Control)) != 0)
        {
            base.OnKeyDown(e);
            return;
        }

        if (e.Key == Key.Up)
        {
            ArrowUp?.Invoke();
            return;
        }
        if (e.Key == Key.Down)
        {
            ArrowDown?.Invoke();
            return;
        }
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