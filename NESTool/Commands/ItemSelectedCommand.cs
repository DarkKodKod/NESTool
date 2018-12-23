using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.Commands
{
    public abstract class ItemSelectedCommand : Command
    {
        protected ProjectItem ItemSeleceted = null;

        public ItemSelectedCommand()
        {
            SignalManager.Get<ProjectItemSelectedSignal>().AddListener(OnProjectItemSelected);
            SignalManager.Get<ProjectItemUnselectedSignal>().AddListener(OnProjectItemUnselected);
        }

        public override void Deactivate()
        {
            SignalManager.Get<ProjectItemSelectedSignal>().RemoveListener(OnProjectItemSelected);
            SignalManager.Get<ProjectItemUnselectedSignal>().RemoveListener(OnProjectItemUnselected);
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
