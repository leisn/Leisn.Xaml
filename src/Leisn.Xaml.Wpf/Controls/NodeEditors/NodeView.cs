// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Leisn.NodeEditor;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Controls;

[TemplatePart(Name = "PART_TitleBar")]
public class NodeView : Control, INode
{
    private FrameworkElement _titleBar = null!;
    static NodeView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeView), new FrameworkPropertyMetadata(typeof(NodeView)));
    }

    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(NodeView), new PropertyMetadata(null));

    public ISlot[] Inputs
    {
        get { return (ISlot[])GetValue(InputsProperty); }
        set { SetValue(InputsProperty, value); }
    }
    public static readonly DependencyProperty InputsProperty =
        DependencyProperty.Register("Inputs", typeof(ISlot[]), typeof(NodeView), new PropertyMetadata(null));

    public ISlot[] Outputs
    {
        get { return (ISlot[])GetValue(OutputsProperty); }
        set { SetValue(OutputsProperty, value); }
    }
    public static readonly DependencyProperty OutputsProperty =
        DependencyProperty.Register("Outputs", typeof(ISlot[]), typeof(NodeView), new PropertyMetadata(null));

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _titleBar = (FrameworkElement)GetTemplateChild("PART_TitleBar");
        _titleBar.MouseDown += TitleBar_MouseDown;
        _titleBar.MouseMove += TitleBar_MouseMove;
        _titleBar.MouseUp += TitleBar_MouseUp;
    }

    #region title bar events
    private bool _isDragging;
    private Point _dragStartPoint;
    private Point _locationStart;
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
            return;
        _isDragging = true;
        UIElement depend = Window.GetWindow(this) ?? (UIElement)Parent ?? this;
        _dragStartPoint = e.GetPosition(depend);
        _locationStart = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        _titleBar.CaptureMouse();
        e.Handled = true;

    }
    private void TitleBar_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging || e.LeftButton != MouseButtonState.Pressed)
        {
            TitleBar_MouseUp(sender, null!);
            return;
        }
        _titleBar.Cursor = Cursors.ScrollAll;
        UIElement depend = Window.GetWindow(this) ?? (UIElement)Parent ?? this;
        var offset = e.GetPosition(depend) - _dragStartPoint;
        var newLocation = _locationStart + offset;
        Canvas.SetLeft(this, newLocation.X);
        Canvas.SetTop(this, newLocation.Y);
        e.Handled = true;
    }
    private void TitleBar_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        _titleBar.ReleaseMouseCapture();
        _titleBar.Cursor = Cursors.Arrow;
    }
    #endregion
}
