using NESTool.History.HistoryActions;

namespace NESTool.History
{
    public class Memento
    {
        public IHistoryAction Action { get; }

        public Memento(IHistoryAction action)
        {
            Action = action;
        }
    }
}
