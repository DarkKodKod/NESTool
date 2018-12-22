using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.Commands
{
    public class PasteElementCommand : Command
    {
        private ProjectItem ItemSeleceted = null;

        public PasteElementCommand()
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
            var newItem = ClipboardManager.GetData() as ProjectItem;

            if (newItem != null)
            {
                SignalManager.Get<PasteElementSignal>().Dispatch(ItemSeleceted, newItem);
            }
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
