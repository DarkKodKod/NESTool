using ArchitectureLibrary.Commands;
using ArchitectureLibrary.History;

namespace NESTool.Commands
{
    public class RedoCommand : Command
    {
        public override bool CanExecute(object parameter) => HistoryManager.IsRedoPossible();

        public override void Execute(object parameter)
        {
            HistoryManager.Redo();
        }
    }
}
