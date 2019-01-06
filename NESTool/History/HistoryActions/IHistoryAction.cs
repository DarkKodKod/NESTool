namespace NESTool.History.HistoryActions
{
    public interface IHistoryAction
    {
        void Undo();
        void Redo();
    }
}
