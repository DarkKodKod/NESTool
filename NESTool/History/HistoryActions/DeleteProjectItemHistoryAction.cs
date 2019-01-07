﻿using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.History.HistoryActions
{
    public class DeleteProjectItemHitoryAction : IHistoryAction
    {
        private readonly ProjectItem _item;

        public DeleteProjectItemHitoryAction(ProjectItem item)
        {
            _item = item;
        }

        public void Redo()
        {
            ProjectItemFileSystem.DeteElement(_item);

            SignalManager.Get<DeleteElementSignal>().Dispatch(_item);
        }

        public void Undo()
        {
            SignalManager.Get<PasteElementSignal>().Dispatch(_item.Parent, _item);

            ProjectItemFileSystem.CreateElement(_item, _item.FileHandler.Path, _item.DisplayName);
        }
    }
}
