using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NESTool.Commands
{
    public class PreviewMouseMoveCommand : Command
    {
        public override void Execute(object parameter)
        {
            MouseEventArgs mouseEvent = parameter as MouseEventArgs;

            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)mouseEvent.OriginalSource);

                MouseMoveVO vo = new MouseMoveVO
                {
                    Position = mouseEvent.GetPosition(treeViewItem),
                    OriginalSource = mouseEvent.OriginalSource,
                    Sender = mouseEvent.Source
                };

                SignalManager.Get<MouseMoveSignal>().Dispatch(vo);
            }
        }
    }
}
