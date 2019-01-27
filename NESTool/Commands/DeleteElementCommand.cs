using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.HistoryActions;
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

            if (ItemSelected.IsRoot)
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

            SignalManager.Get<RegisterHistoryActionSignal>().Dispatch(new DeleteProjectItemHitoryAction(ItemSelected));

            ProjectItemFileSystem.DeteElement(ItemSelected);

            SignalManager.Get<DeleteElementSignal>().Dispatch(ItemSelected);
        }
    }
}
