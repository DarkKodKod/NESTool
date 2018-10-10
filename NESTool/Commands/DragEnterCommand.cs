using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class DragEnterCommand : Command
    {
        public override void Execute(object parameter)
        {
            var dragEvent = parameter as DragEventArgs;

            TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

            if (treeViewItem != null)
            {
                dragEvent.Effects = DragDropEffects.Move;

                SignalManager.Get<InitializeAdornersSignal>().Dispatch(treeViewItem, dragEvent);
            }

            dragEvent.Handled = true;
        }
    }
}
