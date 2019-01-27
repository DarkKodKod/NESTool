using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.History.HistoryActions;
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
            if (ClipboardManager.IsEmpty() || ItemSelected == null)
            {
                return false;
            }

            // Only is possible the paste element if is in a place with the same type
            if (ClipboardManager.GetData() is ProjectItem clipbr && clipbr.Type != ItemSelected.Type)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            if (ItemSelected.FileHandler == null)
            {
                return;
            }

            if (ClipboardManager.GetData() is ProjectItem newItem)
            {
                string newItemPath = string.Empty;
                string name = string.Empty;

                if (ItemSelected.IsFolder)
                {
                    newItemPath = Path.Combine(ItemSelected.FileHandler.Path, ItemSelected.FileHandler.Name);
                }
                else
                {
                    newItemPath = ItemSelected.FileHandler.Path;
                }

                if (newItem.IsFolder)
                {
                    name = ProjectItemFileSystem.GetValidFolderName(newItemPath, newItem.DisplayName);
                }
                else
                {
                    string extension = Util.GetExtensionByType(ItemSelected.Type);

                    name = ProjectItemFileSystem.GetValidFileName(newItemPath, newItem.DisplayName, extension);
                }

                newItem.DisplayName = name;
                newItem.IsLoaded = true;

                SignalManager.Get<RegisterHistoryActionSignal>().Dispatch(new PasteProjectItemHistoryAction(newItem));

                SignalManager.Get<PasteElementSignal>().Dispatch(ItemSelected, newItem);

                ProjectItemFileSystem.CreateElement(newItem, newItemPath, name);
            }
        }
    }
}
