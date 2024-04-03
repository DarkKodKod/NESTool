using ArchitectureLibrary.Commands;
using ArchitectureLibrary.History;

namespace NESTool.Commands;

public class UndoCommand : Command
{
    public override bool CanExecute(object parameter) => HistoryManager.IsUndoPossible();

    public override void Execute(object parameter)
    {
        HistoryManager.Undo();
    }
}
