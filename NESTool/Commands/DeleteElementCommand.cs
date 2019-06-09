using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.HistoryActions;
using NESTool.Signals;
using System.Windows;

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

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this element?", "Delete", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                SignalManager.Get<RegisterHistoryActionSignal>().Dispatch(new DeleteProjectItemHitoryAction(ItemSelected));

                ProjectItemFileSystem.DeteElement(ItemSelected);

                SignalManager.Get<DeleteElementSignal>().Dispatch(ItemSelected);
            }
        }
    }
}
