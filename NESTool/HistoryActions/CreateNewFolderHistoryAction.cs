using ArchitectureLibrary.History;
using NESTool.ViewModels;

namespace NESTool.HistoryActions
{
    public class CreateNewFolderHistoryAction : IHistoryAction
    {
        private readonly ProjectItem _item;

        public CreateNewFolderHistoryAction(ProjectItem item)
        {
            _item = item;
        }

        public void Redo()
        {
            //
        }

        public void Undo()
        {
            //
        }
    }
}
