using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System.Windows;

namespace NESTool.Commands;

public class QueryContinueDragCommand : Command
{
    public override void Execute(object parameter)
    {
        QueryContinueDragEventArgs dragEvent = parameter as QueryContinueDragEventArgs;

        if (dragEvent.EscapePressed)
        {
            dragEvent.Action = DragAction.Cancel;

            SignalManager.Get<DetachAdornersSignal>().Dispatch();

            dragEvent.Handled = true;
        }
    }
}
