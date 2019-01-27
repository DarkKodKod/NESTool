using ArchitectureLibrary.History;
using NESTool.ViewModels;

namespace NESTool.HistoryActions
{
    public class CreateNewElementHistoryAction : IHistoryAction
    {
        private readonly ProjectItem _item;

        public CreateNewElementHistoryAction(ProjectItem item)
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
