using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands;

public class OpenProjectPropertiesCommand : Command
{
    public override bool CanExecute(object parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        object[] values = (object[])parameter;
        Window window = (Window)values[0];
        string projectName = (string)values[1];

        if (string.IsNullOrEmpty(projectName))
        {
            return false;
        }

        return true;
    }

    public override void Execute(object parameter)
    {
        object[] values = (object[])parameter;
        Window window = (Window)values[0];
        string projectName = (string)values[1];

        ProjectPropertiesDialog dialog = new ProjectPropertiesDialog
        {
            Owner = window
        };
        dialog.ShowDialog();
    }
}
