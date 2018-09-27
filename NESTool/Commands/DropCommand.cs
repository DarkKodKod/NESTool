using NESTool.Architecture.Commands;
using NESTool.Utils;
using NESTool.ViewModels.ProjectItems;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class DropCommand : Command
    {
        public override void Execute(object parameter)
        {
            var mouseEvent = parameter as DragEventArgs;

            if (mouseEvent.Data.GetDataPresent(typeof(ProjectItem)))
            {
                var folderViewModel = mouseEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

                var treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)mouseEvent.OriginalSource);

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
