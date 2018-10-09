using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class OpenProjectPropertiesCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            var values = (object[])parameter;
            var window = (Window)values[0];
            var projectName = (string)values[1];

            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var values = (object[])parameter;
            var window = (Window)values[0];
            var projectName = (string)values[1];

            ProjectPropertiesDialog dialog = new ProjectPropertiesDialog();
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
