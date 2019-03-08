using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;
using System.Windows;

namespace NESTool.Commands
{
    public class TreeviewSelectedItemChangedCommand : Command
    {
        public override void Execute(object parameter)
        {
            RoutedPropertyChangedEventArgs<object> selectedItemChangedEvent = parameter as RoutedPropertyChangedEventArgs<object>;

            ProjectItem item = selectedItemChangedEvent.NewValue as ProjectItem;

            if (item != null)
            {
                SignalManager.Get<LoadProjectItemSignal>().Dispatch(item);
            }
        }
    }
}
