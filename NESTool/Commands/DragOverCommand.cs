using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class DragOverCommand : Command
    {
        private DateTime _startTime;
        private string _folderId = string.Empty;

        public override bool CanExecute(object parameter)
        {
            DragEventArgs dragEvent = parameter as DragEventArgs;

            ProjectItem draggingItem = dragEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

            TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

            if (treeViewItem.DataContext is ProjectItem item && item.Type != draggingItem.Type)
            {
                dragEvent.Handled = true;

                SignalManager.Get<DetachAdornersSignal>().Dispatch();

                dragEvent.Effects = DragDropEffects.None;

                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var dragEvent = parameter as DragEventArgs;

            TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

            if (treeViewItem != null)
            {
                if (treeViewItem.DataContext is ProjectItem element)
                {
                    if (element.IsFolder && !treeViewItem.IsExpanded)
                    {
                        if (_folderId != element.FileHandler.Meta.GUID)
                        {
                            _folderId = element.FileHandler.Meta.GUID;
                            _startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            int milliseconds = Convert.ToInt32((DateTime.UtcNow - _startTime).TotalMilliseconds);

                            if (milliseconds >= 1000)
                            {
                                treeViewItem.IsExpanded = true;
                            }
                        }
                    }
                }

                SignalManager.Get<UpdateAdornersSignal>().Dispatch(treeViewItem, dragEvent);
            }

            dragEvent.Handled = true;
        }
    }
}
