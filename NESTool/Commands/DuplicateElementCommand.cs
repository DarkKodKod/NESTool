using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;

namespace NESTool.Commands
{
    public class DuplicateElementCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ItemSeleceted == null)
            {
                return false;
            }

            if (ItemSeleceted.Root || ItemSeleceted.IsFolder)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            if (ItemSeleceted == null)
            {
                return;
            }

            ProjectItem newItem = new ProjectItem(ItemSeleceted.GetContent());

             string name = ProjectItemFileSystem.GetValidFileName(
                newItem.ParentFolder, 
                newItem.DisplayName, 
                Util.GetExtensionByType(ItemSeleceted.Type));

            newItem.DisplayName = name;

            SignalManager.Get<PasteElementSignal>().Dispatch(ItemSeleceted, newItem);

            ProjectItemFileSystem.CreateFileElement(ref newItem);
        }
    }
}
