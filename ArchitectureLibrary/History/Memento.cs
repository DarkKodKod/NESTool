namespace ArchitectureLibrary.History
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
