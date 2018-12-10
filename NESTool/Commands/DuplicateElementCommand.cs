using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.Commands
{
    public class DuplicateElementCommand : Command
    {
        private ProjectItem ItemSeleceted = null;

        public DuplicateElementCommand()
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

            var item = ItemSeleceted as ProjectItem;

            if (item.Root || item.IsFolder)
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

            SignalManager.Get<PasteFileSignal>().Dispatch(ItemSeleceted, newItem);
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
