namespace NESTool.History.HistoryActions
{
    public class DeleteProjectItemHitoryAction : IHistoryAction
    {
        private readonly string _path = string.Empty;

        public DeleteProjectItemHitoryAction(string path)
        {
            _path = path;
        }

        public void Redo()
        {
            // Delete file from path _path;
        }

        public void Undo()
        {
            // Restore deleted file from path _path
        }
    }
}
