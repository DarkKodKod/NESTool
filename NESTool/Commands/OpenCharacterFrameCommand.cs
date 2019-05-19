using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class OpenCharacterFrameCommand : Command
    {
        public override void Execute(object parameter)
        {
            SignalManager.Get<OpenCharacterFrameSignal>().Dispatch();
        }
    }
}
