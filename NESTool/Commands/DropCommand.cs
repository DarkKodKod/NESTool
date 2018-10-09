using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels.ProjectItems;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class DropCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            var dragEvent = parameter as DragEventArgs;

            if (dragEvent.Data.GetDataPresent(typeof(ProjectFolder)))
            {
                var folder = dragEvent.Data.GetData(typeof(ProjectFolder)) as ProjectFolder;

                if (folder.Root)
                {
                    return false;
                }
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var dragEvent = parameter as DragEventArgs;

            if (dragEvent.Data.GetDataPresent(typeof(ProjectItem)))
            {
                var folderViewModel = dragEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

                var treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

                if (treeViewItem == null)
                {
                    return;
                }

                var dropTarget = treeViewItem.Header as ProjectItem;

                if (dropTarget == null || folderViewModel == null)
                {
                    return;
                }

                //folderViewModel.Parent = dropTarget;
            }

            SignalManager.Get<DetachAdornersSignal>().Dispatch();

            dragEvent.Handled = true;
        }
    }
}
