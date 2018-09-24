using NESTool.Architecture.Commands;
using NESTool.Architecture.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class OpenProjectPropertiesCommand : Command
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
            SignalManager.Get<BuildProjectSuccessSignal>().Dispatch();
        }
    }
}
