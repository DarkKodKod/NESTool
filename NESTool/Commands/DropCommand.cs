﻿using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
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

                if (draggingObject.IsFolder)
                {
                    name = ProjectItemFileSystem.GetValidFolderName(destinationFolder, draggingObject.DisplayName);
                }
                else
                {
                    string extension = Util.GetExtensionByType(draggingObject.Type);

                    name = ProjectItemFileSystem.GetValidFileName(destinationFolder, draggingObject.DisplayName, extension);
                }

                draggingObject.DisplayName = name;

                SignalManager.Get<DropElementSignal>().Dispatch(dropTarget, draggingObject);
                SignalManager.Get<MoveElementSignal>().Dispatch(dropTarget, draggingObject);
            }

            dragEvent.Effects = DragDropEffects.None;
        }
    }
}
