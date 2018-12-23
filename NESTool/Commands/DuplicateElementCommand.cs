using ArchitectureLibrary.Signals;
using NESTool.Signals;
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

            SignalManager.Get<PasteElementSignal>().Dispatch(ItemSeleceted, newItem);
        }
    }
}
