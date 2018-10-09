using ArchitectureLibrary.Commands;
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
            var mouseEvent = parameter as DragEventArgs;

            if (mouseEvent.Data.GetDataPresent(typeof(ProjectFolder)))
            {
                var folder = mouseEvent.Data.GetData(typeof(ProjectFolder)) as ProjectFolder;

                if (folder.Root)
                {
                    return false;
                }
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var mouseEvent = parameter as DragEventArgs;

            if (mouseEvent.Data.GetDataPresent(typeof(ProjectItem)))
            {
                var folderViewModel = mouseEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

                var treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)mouseEvent.OriginalSource);

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
        }
    }
}
