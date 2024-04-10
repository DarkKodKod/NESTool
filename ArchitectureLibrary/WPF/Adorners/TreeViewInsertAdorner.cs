using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArchitectureLibrary.WPF.Adorners;

public class TreeViewInsertAdorner : Adorner
{
    public bool IsTopHalf { get; set; }

    private AdornerLayer _adornerLayer;
    private Pen _pen;

    public TreeViewInsertAdorner(bool isTopHalf, UIElement adornedElement, AdornerLayer adornerLayer) : base(adornedElement)
    {
        IsHitTestVisible = false;

        IsTopHalf = isTopHalf;

        _adornerLayer = adornerLayer;
        _adornerLayer.Add(this);

        _pen = new Pen(new SolidColorBrush(Colors.Red), 3.0);

        DoubleAnimation animation = new DoubleAnimation(0.5, 1, new Duration(TimeSpan.FromSeconds(0.5)))
        {
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        _pen.Brush.BeginAnimation(Brush.OpacityProperty, animation);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        DetermineHorizontalLinePoints(out Point startPoint, out Point endPoint);

        drawingContext.DrawLine(_pen, startPoint, endPoint);
    }

    private void DetermineHorizontalLinePoints(out Point startPoint, out Point endPoint)
    {
        double width = AdornedElement.RenderSize.Width;
        double height = AdornedElement.RenderSize.Height;

        if (!IsTopHalf)
        {
            startPoint = new Point(0, height);
            endPoint = new Point(width, height);
        }
        else
        {
            startPoint = new Point(0, 0);
            endPoint = new Point(width, 0);
        }
    }

    public void Destroy()
    {
        _adornerLayer.Remove(this);
    }
}
