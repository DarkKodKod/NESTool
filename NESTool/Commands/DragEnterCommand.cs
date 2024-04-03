using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands;

public class DragEnterCommand : Command
{
    public override bool CanExecute(object parameter)
    {
        DragEventArgs dragEvent = parameter as DragEventArgs;

        ProjectItem draggingItem = dragEvent.Data.GetData(typeof(ProjectItem)) as ProjectItem;

        TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

        if (treeViewItem == null)
        {
            return false;
        }

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
        DragEventArgs dragEvent = parameter as DragEventArgs;

        TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)dragEvent.OriginalSource);

        if (treeViewItem != null)
        {
            dragEvent.Effects = DragDropEffects.Move;

            SignalManager.Get<InitializeAdornersSignal>().Dispatch(treeViewItem, dragEvent);
        }

        dragEvent.Handled = true;
    }
}
