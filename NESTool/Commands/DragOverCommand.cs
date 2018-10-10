using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class DragOverCommand : Command
    {
        public override void Execute(object parameter)
        {
            var dragEvent = parameter as DragEventArgs;

            TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

            if (treeViewItem != null)
            {
                SignalManager.Get<UpdateAdornersSignal>().Dispatch(treeViewItem, dragEvent);
            }

            dragEvent.Handled = true;
        }
    }
}
