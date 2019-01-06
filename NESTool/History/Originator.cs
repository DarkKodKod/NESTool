using NESTool.History.HistoryActions;

namespace NESTool.History
{
    public class Originator
    {
        public Memento CreateMemento(IHistoryAction action)
        {
            return new Memento(action);
        }

        public void ExecuteUndo(Memento memento)
        {
            memento.Action.Undo();
        }

        public void ExecuteRedo(Memento memento)
        {
            memento.Action.Redo();
        }
    }
}
