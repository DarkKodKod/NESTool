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

namespace NESTool.Commands;

public class DropCommand : Command
{
    public override bool CanExecute(object? parameter)
    {
        SignalManager.Get<DetachAdornersSignal>().Dispatch();

        DragEventArgs? dragEvent = parameter as DragEventArgs;

        if (dragEvent?.Data.GetData(typeof(ProjectItem)) is not ProjectItem item || item.IsRoot)
        {
            return false;
        }

        return true;
    }

    public override void Execute(object? parameter)
    {
        if (parameter is not DragEventArgs dragEvent)
            return;

        dragEvent.Handled = true;

        if (dragEvent.Data.GetDataPresent(typeof(ProjectItem)))
        {
            TreeViewItem? treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

            if (treeViewItem == null)
            {
                return;
            }

            if (treeViewItem.Header is not ProjectItem dropTarget || dragEvent.Data.GetData(typeof(ProjectItem)) is not ProjectItem draggingObject)
            {
                return;
            }

            // It is important to deselect the dragged object to select it again when the whole process finishes
            draggingObject.IsSelected = false;

            string name;
            string destinationFolder;

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
