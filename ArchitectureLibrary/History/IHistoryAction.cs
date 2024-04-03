namespace ArchitectureLibrary.History;

public interface IHistoryAction
{
    void Undo();
    void Redo();
}
