using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.History.HistoryActions
{
    public class MoveProjectItemHistoryAction : IHistoryAction
    {
        private readonly ProjectItem _item;

        public MoveProjectItemHistoryAction(ProjectItem item)
        {
            _item = item;
        }

        public void Redo()
        {
            //SignalManager.Get<PasteElementSignal>().Dispatch(_item.Parent, _item);

            //ProjectItemFileSystem.CreateElement(_item, _item.FileHandler.Path, _item.DisplayName);
        }

        public void Undo()
        {
            //ProjectItemFileSystem.DeteElement(_item);

            //SignalManager.Get<DeleteElementSignal>().Dispatch(_item);
        }
    }
}
