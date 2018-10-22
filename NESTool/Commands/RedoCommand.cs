using ArchitectureLibrary.Commands;

namespace NESTool.Commands
{
    public class RedoCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return false;
        }

        public override void Execute(object parameter)
        {
            //
        }
    }
}
