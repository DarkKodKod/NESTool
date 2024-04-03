using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows.Input;

namespace NESTool.Commands;

public class PreviewMouseWheelCommand : Command
{
    public override void Execute(object parameter)
    {
        MouseWheelEventArgs wheelEvent = parameter as MouseWheelEventArgs;

        MouseWheelVO vo = new()
        {
            Delta = wheelEvent.Delta,
            OriginalSource = wheelEvent.OriginalSource,
            Sender = wheelEvent.Source
        };

        SignalManager.Get<MouseWheelSignal>().Dispatch(vo);
    }
}
