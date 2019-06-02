using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Signals;

namespace ArchitectureLibrary.History
{
    public static class HistoryManager
    {
        static Caretaker _caretaker = new Caretaker();
        static Originator _mementoOriginator = new Originator();

        public static void Initialize()
        {
            SignalManager.Get<RegisterHistoryActionSignal>().AddListener(OnRegisterHistoryAction);
        }

        public static void Undo()
        {
            Memento memento = _caretaker.GetUndoMemento();

            if (memento != null)
            {
                _mementoOriginator.ExecuteUndo(memento);
            }
        }

        public static void Redo()
        {
            Memento memento = _caretaker.GetRedoMemento();

            if (memento != null)
            {
                _mementoOriginator.ExecuteRedo(memento);
            }
        }

        public static bool IsUndoPossible()
        {
            return _caretaker.IsUndoPossible();
        }

        public static bool IsRedoPossible()
        {
            return _caretaker.IsRedoPossible();
        }

        private static void OnRegisterHistoryAction(IHistoryAction action)
        {
            Memento memento = _mementoOriginator.CreateMemento(action);

            _caretaker.InsertMementoForUndoRedo(memento);
        }
    }
}
