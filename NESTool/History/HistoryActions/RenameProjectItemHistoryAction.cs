﻿using NESTool.ViewModels;

namespace NESTool.History.HistoryActions
{
    public class RenameProjectItemHistoryAction : IHistoryAction
    {
        private readonly ProjectItem _item;

        public RenameProjectItemHistoryAction(ProjectItem item)
        {
            _item = item;
        }

        public void Redo()
        {
            //
        }

        public void Undo()
        {
            //
        }
    }
}
