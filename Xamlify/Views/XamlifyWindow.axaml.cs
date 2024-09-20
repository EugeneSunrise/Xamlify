using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Xamlify.ViewModels;

namespace Xamlify.Views;

public partial class XamlifyWindow : Window
{
    private bool _mouseDownForWindowMoving = false;
    private PointerPoint _originalPoint;
    public XamlifyWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }
    
    private void DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Link);

        if (!e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var paths = e.Data.GetFileNames();
            if (paths is { })
            {
                if (DataContext is XamlifyWindowViewModel vm)
                {
                    try
                    {
                        vm.Drop(paths);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }
    }
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (WindowState is WindowState.Maximized or WindowState.FullScreen) return;

        _mouseDownForWindowMoving = true;
        _originalPoint = e.GetCurrentPoint(this);
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_mouseDownForWindowMoving) return;

        PointerPoint currentPoint = e.GetCurrentPoint(this);
        Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
            Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _mouseDownForWindowMoving = false;
    }
}