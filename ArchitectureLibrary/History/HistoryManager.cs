using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Signals;

namespace ArchitectureLibrary.History;

public static class HistoryManager
{
    static readonly Caretaker _caretaker = new();

    public static void Initialize()
    {
        SignalManager.Get<RegisterHistoryActionSignal>().Listener += OnRegisterHistoryAction;
    }

    public static void Undo()
    {
        Memento? memento = _caretaker.GetUndoMemento();

        if (memento != null)
        {
            Originator.ExecuteUndo(memento);
        }
    }

    public static void Redo()
    {
        Memento? memento = _caretaker.GetRedoMemento();

        if (memento != null)
        {
            Originator.ExecuteRedo(memento);
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
        Memento memento = Originator.CreateMemento(action);

        _caretaker.InsertMementoForUndoRedo(memento);
    }
}
