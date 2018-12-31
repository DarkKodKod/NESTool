using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class DeleteElementCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ItemSelected == null)
            {
                return false;
            }

            if (ItemSelected.Root)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            if (ItemSelected == null)
            {
                return;
            }

            ProjectItemFileSystem.DeteFile(ItemSelected.FileHandler);

            SignalManager.Get<DeleteElementSignal>().Dispatch(ItemSelected);
        }
    }
}
