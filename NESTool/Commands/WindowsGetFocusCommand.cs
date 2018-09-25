using NESTool.Architecture.Commands;
using NESTool.Architecture.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class WindowsGetFocusCommand : Command
    {
        public override void Execute(object parameter)
        {
            SignalManager.Get<WindowGetFocusSignal>().Dispatch();
        }
    }
}
