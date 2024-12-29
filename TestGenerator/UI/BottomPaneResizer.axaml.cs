using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace TestGenerator.UI;

public partial class BottomPaneResizer : UserControl
{
    private Border? _pane;
    private Point? _lastPoint;
    
    public BottomPaneResizer()
    {
        InitializeComponent();
    }

    private Border FindPane()
    {
        StyledElement? elem = this;
        while (elem != null)
        {
            elem = elem.Parent;
            if (elem is Border border)
                return border;
        }

        throw new Exception("Pane not found");
    }
    
    private static readonly Cursor SplitterCursor = new(StandardCursorType.SizeNorthSouth);

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Cursor = SplitterCursor;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _pane ??= FindPane();
        e.Handled = true;
        _lastPoint = e.GetPosition(null);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_pane == null || _lastPoint == null)
            return;
        
        var mouseMovement = (Point)(e.GetPosition(null) - _lastPoint);
        var delta = mouseMovement.Y;

        _pane.Height -= delta;
        _lastPoint = e.GetPosition(null);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        e.Handled = _lastPoint != null;
        _lastPoint = null;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _lastPoint = null;
        base.OnPointerCaptureLost(e);
    }
}