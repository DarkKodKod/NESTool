using ArchitectureLibrary.Commands;

namespace NESTool.Commands
{
    public class MovePaletteDownCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;

            if (values[0] == null)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            //
        }
    }
}
