namespace ArchitectureLibrary.History;

public class Originator
{
    public static Memento CreateMemento(IHistoryAction action)
    {
        return new Memento(action);
    }

    public static void ExecuteUndo(Memento memento)
    {
        memento.Action.Undo();
    }

    public static void ExecuteRedo(Memento memento)
    {
        memento.Action.Redo();
    }
}
