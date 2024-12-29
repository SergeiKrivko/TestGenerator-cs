using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace TestGenerator.UI;

public partial class SplitViewResizer : UserControl
{
    /// <summary>
    /// Initialize the data needed for resizing.
    /// </summary>
    private void InitializeData()
    {
        SplitView? splitView = null;

        var parent = this.GetLogicalParent();
        while (parent != null)
        {
            if (parent is SplitView sv)
            {
                splitView = sv;
                break;
            }

            parent = parent.GetLogicalParent();
        }

        // If not in a grid or can't resize, do nothing.
        if (splitView != null)
        {
            // Setup data used for resizing.
            _resizeData = new ResizeData
            {
                SplitView = splitView,
                OriginalPaneLength = splitView.OpenPaneLength,
            };
        }
    }

    private static readonly Cursor SColumnSplitterCursor = new(StandardCursorType.SizeWestEast);

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Cursor = SColumnSplitterCursor;
    }

    private ResizeData? _resizeData;
    private Point? _lastPoint;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        e.Handled = true;
        e.PreventGestureRecognition();

        _lastPoint = e.GetPosition(null);

        if (_resizeData == null)
        {
            InitializeData();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Debug.WriteLine("SplitViewResizer Render");
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_lastPoint == null)
        {
            return;
        }

        Debug.WriteLine("SplitViewResizer Move: {0}", e.GetPosition(null));

        if (_resizeData != null)
        {
            var mouseMovement = (Point)(e.GetPosition(null) - _lastPoint);
            var delta = mouseMovement.X;

            var targetLength = Math.Max(Math.Min(_resizeData.OriginalPaneLength + delta, 1000), 200);
            _resizeData.SplitView.OpenPaneLength = targetLength;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        e.Handled = _lastPoint != null;
        _lastPoint = null;
        _resizeData = null;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (_lastPoint.HasValue)
        {
            _lastPoint = null;
            _resizeData = null;
        }

        base.OnPointerCaptureLost(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        if (_resizeData != null)
        {
            CancelResize();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                if (_resizeData != null)
                {
                    CancelResize();
                    e.Handled = true;
                }

                break;
        }
    }

    /// <summary>
    /// Cancels the resize operation.
    /// </summary>
    private void CancelResize()
    {
        if (_resizeData != null)
        {
            _resizeData.SplitView.OpenPaneLength = _resizeData.OriginalPaneLength;
            _resizeData = null;
        }
    }

    /// <summary>
    /// Stores data during the resizing operation.
    /// </summary>
    private class ResizeData
    {
        public SplitView SplitView;
        public double OriginalPaneLength;
    }
}