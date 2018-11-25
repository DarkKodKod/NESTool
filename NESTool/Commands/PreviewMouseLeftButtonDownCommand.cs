using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NESTool.Commands
{
    public class PreviewMouseLeftButtonDownCommand : Command
    {
        public override void Execute(object parameter)
        {
            var mouseEvent = parameter as MouseButtonEventArgs;

            TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)mouseEvent.OriginalSource);

            var position = mouseEvent.GetPosition(treeViewItem);

            SignalManager.Get<MouseLeftButtonDownSignal>().Dispatch(position);
        }
    }
}
