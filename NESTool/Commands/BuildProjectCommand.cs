using ArchitectureLibrary.Commands;

namespace NESTool.Commands
{
    public class BuildProjectCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            string projectName = parameter as string;

            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }

            return false;
        }

        public override void Execute(object parameter)
        {
            //
        }
    }
}
