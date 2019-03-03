using ArchitectureLibrary.Commands;
using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.HistoryActions;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.IO;
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
                ProjectItem draggingObject = dragEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

                TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

                if (treeViewItem == null)
                {
                    return;
                }

                if (!(treeViewItem.Header is ProjectItem dropTarget) || draggingObject == null)
                {
                    return;
                }

                // It is important to deselect the dragged object to select it again when the whole process finishes
                draggingObject.IsSelected = false;

                string name = string.Empty;
                string destinationFolder = string.Empty;

                if (dropTarget.IsFolder)
                {
                    destinationFolder = Path.Combine(dropTarget.FileHandler.Path, dropTarget.FileHandler.Name);
                }
                else
                {
                    destinationFolder = dropTarget.FileHandler.Path;
                }

                if (destinationFolder == draggingObject.FileHandler.Path)
                {
                    return;
                }

                if (draggingObject.IsFolder)
                {
                    name = ProjectItemFileSystem.GetValidFolderName(destinationFolder, draggingObject.DisplayName);
                }
                else
                {
                    string extension = Util.GetExtensionByType(draggingObject.Type);

                    name = ProjectItemFileSystem.GetValidFileName(destinationFolder, draggingObject.DisplayName, extension);
                }

                string oldName = draggingObject.DisplayName;

                draggingObject.RenamedFromAction = true;
                draggingObject.DisplayName = name;

                SignalManager.Get<RegisterHistoryActionSignal>().Dispatch(
                    new MoveProjectItemHistoryAction(dropTarget, draggingObject, draggingObject.Parent, name, oldName)
                    );

                SignalManager.Get<DropElementSignal>().Dispatch(dropTarget, draggingObject);
                SignalManager.Get<MoveElementSignal>().Dispatch(dropTarget, draggingObject);
            }

            dragEvent.Effects = DragDropEffects.None;
        }
    }
}
