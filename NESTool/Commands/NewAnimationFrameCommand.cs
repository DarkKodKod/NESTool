using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class NewAnimationFrameCommand : Command
    {
        public override void Execute(object parameter)
        {
            string tabID = parameter as string;

            SignalManager.Get<NewAnimationFrameSignal>().Dispatch(tabID);
        }
    }
}
