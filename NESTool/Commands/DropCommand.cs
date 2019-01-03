using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class DropCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            SignalManager.Get<DetachAdornersSignal>().Dispatch();

            DragEventArgs dragEvent = parameter as DragEventArgs;

            ProjectItem item = dragEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

            if (item.IsRoot)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            DragEventArgs dragEvent = parameter as DragEventArgs;

            dragEvent.Handled = true;

            if (dragEvent.Data.GetDataPresent(typeof(ProjectItem)))
            {
                ProjectItem folderViewModel = dragEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

                TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

                if (treeViewItem == null)
                {
                    return;
                }

                ProjectItem dropTarget = treeViewItem.Header as ProjectItem;

                if (dropTarget == null || folderViewModel == null)
                {
                    return;
                }

                //folderViewModel.Parent = dropTarget;
            }

            dragEvent.Effects = DragDropEffects.None;
        }
    }
}
