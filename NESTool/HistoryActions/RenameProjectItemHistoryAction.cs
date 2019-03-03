using ArchitectureLibrary.History;
using NESTool.ViewModels;

namespace NESTool.HistoryActions
{
    public class RenameProjectItemHistoryAction : IHistoryAction
    {
        private readonly ProjectItem _item;
        private readonly string _oldName;
        private readonly string _newName;

        public RenameProjectItemHistoryAction(ProjectItem item, string oldName)
        {
            _item = item;
            _oldName = oldName;
            _newName = item.DisplayName;
        }

        public void Redo()
        {
            _item.RenamedFromAction = true;
            _item.DisplayName = _newName;
        }

        public void Undo()
        {
            _item.RenamedFromAction = true;
            _item.DisplayName = _oldName;
        }
    }
}
