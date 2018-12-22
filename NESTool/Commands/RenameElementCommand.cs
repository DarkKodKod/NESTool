using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.Commands
{
    public class RenameElementCommand : Command
    {
        private ProjectItem ItemSeleceted = null;

        public RenameElementCommand()
        {
            SignalManager.Get<ProjectItemSelectedSignal>().AddListener(OnProjectItemSelected);
            SignalManager.Get<ProjectItemUnselectedSignal>().AddListener(OnProjectItemUnselected);
        }

        public override void Deactivate()
        {
            SignalManager.Get<ProjectItemSelectedSignal>().RemoveListener(OnProjectItemSelected);
            SignalManager.Get<ProjectItemUnselectedSignal>().RemoveListener(OnProjectItemUnselected);
        }

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
            if (parameter is ProjectItem)
            {
                ItemSeleceted = parameter as ProjectItem;
            }

            if (ItemSeleceted == null)
            {
                return;
            }

            ItemSeleceted.IsInEditMode = true;
        }

        private void OnProjectItemSelected(ProjectItem item)
        {
            ItemSeleceted = item;
        }

        private void OnProjectItemUnselected(ProjectItem item)
        {
            ItemSeleceted = null;
        }
    }
}
