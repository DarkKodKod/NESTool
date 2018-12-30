using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.IO;

namespace NESTool.Commands
{
    public class PasteElementCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ClipboardManager.IsEmpty() || ItemSeleceted == null)
            {
                return false;
            }

            // Only is possible the paste element if is in a place with the same type
            ProjectItem clipbr = ClipboardManager.GetData() as ProjectItem;
            if (clipbr != null && clipbr.Type != ItemSeleceted.Type)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            if (ClipboardManager.GetData() is ProjectItem newItem)
            {
                string newItemPath;
                string name;

                if (ItemSeleceted.IsFolder)
                {
                    newItemPath = ItemSeleceted.FullPath;
                }
                else
                {
                    newItemPath = ItemSeleceted.ParentFolder;
                }

                if (newItem.IsFolder)
                {
                    name = ProjectItemFileSystem.GetValidFolderName(newItemPath, newItem.DisplayName);

                    newItem.FullPath = Path.Combine(newItemPath, name);
                }
                else
                {
                    string extension = Util.GetExtensionByType(ItemSeleceted.Type);

                    name = ProjectItemFileSystem.GetValidFileName(newItemPath, newItem.DisplayName, extension);

                    newItem.FullPath = Path.Combine(newItemPath, name + extension);
                }

                newItem.DisplayName = name;
                newItem.ParentFolder = newItemPath;
                newItem.IsLoaded = true;

                SignalManager.Get<PasteElementSignal>().Dispatch(ItemSeleceted, newItem);

                ProjectItemFileSystem.CreateFileElement(ref newItem);
            }
        }
    }
}
