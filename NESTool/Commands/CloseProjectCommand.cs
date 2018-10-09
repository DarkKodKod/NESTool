using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class CloseProjectCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            var projectName = parameter as string;

            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            SignalManager.Get<CloseProjectSuccessSignal>().Dispatch();
        }
    }
}
