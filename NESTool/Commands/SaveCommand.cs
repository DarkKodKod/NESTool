using NESTool.Architecture.Commands;
using NESTool.Architecture.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class SaveCommand : Command
    {
        public override void Execute(object parameter)
        {
            SignalManager.Get<SaveSuccessSignal>().Dispatch();
        }
    }
}
