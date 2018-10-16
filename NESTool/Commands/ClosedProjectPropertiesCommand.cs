using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class ClosedProjectPropertiesCommand : Command
    {
        public override void Execute(object parameter)
        {
            SignalManager.Get<ClosedProjectPropertiesSignal>().Dispatch();
        }
    }
}
