using ArchitectureLibrary.History;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.HistoryActions;

public class MoveProjectItemHistoryAction : IHistoryAction
{
    private readonly ProjectItem _item;
    private readonly ProjectItem _dropTarget;
    private readonly ProjectItem? _origin;
    private readonly string _newName;
    private readonly string _oldName;

    public MoveProjectItemHistoryAction(ProjectItem dropTarget, ProjectItem item, ProjectItem? origin, string newName, string oldName)
    {
        _item = item;
        _dropTarget = dropTarget;
        _origin = origin;
        _newName = newName;
        _oldName = oldName;
    }

    public void Redo()
    {
        _item.IsSelected = false;

        _item.RenamedFromAction = true;
        _item.DisplayName = _newName;

        SignalManager.Get<DropElementSignal>().Dispatch(_dropTarget, _item);
        SignalManager.Get<MoveElementSignal>().Dispatch(_dropTarget, _item);
    }

    public void Undo()
    {
        _item.IsSelected = false;

        _item.RenamedFromAction = true;
        _item.DisplayName = _oldName;

        if (_origin != null)
        {
            SignalManager.Get<DropElementSignal>().Dispatch(_origin, _item);
            SignalManager.Get<MoveElementSignal>().Dispatch(_origin, _item);
        }
    }
}
