using NESTool.Architecture.Commands;

namespace NESTool.Commands
{
    public class DeleteElementCommand : Command
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
