using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class DeleteElementCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ItemSeleceted == null)
            {
                return false;
            }

            if (ItemSeleceted.Root)
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

            ProjectItemFileSystem.DeteFile(ItemSeleceted.FileHandler);

            SignalManager.Get<DeleteElementSignal>().Dispatch(ItemSeleceted);
        }
    }
}
