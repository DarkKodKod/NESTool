using Microsoft.Xaml.Behaviors;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;

namespace NESTool.Utils.Behaviors;

[SupportedOSPlatform("windows")]
public class RedirectRoutedEventBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty RedirectTargetProperty =
        DependencyProperty.Register("RedirectTarget", typeof(UIElement),
            typeof(RedirectRoutedEventBehavior),
            new PropertyMetadata(null));

    public static readonly DependencyProperty RoutedEventProperty =
        DependencyProperty.Register("RoutedEvent", typeof(RoutedEvent), typeof(RedirectRoutedEventBehavior),
            new PropertyMetadata(null, OnRoutedEventChanged));

    public UIElement RedirectTarget
    {
        get => (UIElement)this.GetValue(RedirectTargetProperty);
        set => this.SetValue(RedirectTargetProperty, value);
    }

    public RoutedEvent RoutedEvent
    {
        get => (RoutedEvent)this.GetValue(RoutedEventProperty);
        set => this.SetValue(RoutedEventProperty, value);
    }

    private static MethodInfo MemberwiseCloneMethod { get; }
        = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

    private static void OnRoutedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RedirectRoutedEventBehavior)d).OnRoutedEventChanged((RoutedEvent)e.OldValue, (RoutedEvent)e.NewValue);
    }

    private void OnRoutedEventChanged(RoutedEvent oldValue, RoutedEvent newValue)
    {
        if (this.AssociatedObject == null)
        {
            return;
        }

        if (oldValue != null)
        {
            this.AssociatedObject.RemoveHandler(oldValue, new RoutedEventHandler(this.EventHandler));
        }

        if (newValue != null)
        {
            this.AssociatedObject.AddHandler(newValue, new RoutedEventHandler(this.EventHandler));
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (this.RoutedEvent != null)
        {
            this.AssociatedObject.AddHandler(this.RoutedEvent, new RoutedEventHandler(this.EventHandler));
        }
    }

    protected override void OnDetaching()
    {
        if (this.RoutedEvent != null)
        {
            this.AssociatedObject.RemoveHandler(this.RoutedEvent, new RoutedEventHandler(this.EventHandler));
        }

        base.OnDetaching();
    }

    private static RoutedEventArgs CloneEvent(RoutedEventArgs e)
    {
        return (RoutedEventArgs)MemberwiseCloneMethod.Invoke(e, null);
    }

    private void EventHandler(object sender, RoutedEventArgs e)
    {
        RoutedEventArgs newEvent = CloneEvent(e);
        e.Handled = true;

        if (this.RedirectTarget != null)
        {
            newEvent.Source = this.RedirectTarget;
            this.RedirectTarget.RaiseEvent(newEvent);
        }
    }
}
