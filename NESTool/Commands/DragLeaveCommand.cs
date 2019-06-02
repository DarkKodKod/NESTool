using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System.Windows;

namespace NESTool.Commands
{
    public class DragLeaveCommand : Command
    {
        public override void Execute(object parameter)
        {
            DragEventArgs dragEvent = parameter as DragEventArgs;

            SignalManager.Get<DetachAdornersSignal>().Dispatch();

            dragEvent.Handled = true;
        }
    }
}
