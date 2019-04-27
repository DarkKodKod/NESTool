using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class OpenBuildProjectCommand : Command
    {
        public override void Execute(object parameter)
        {
            Window window = parameter as Window;

            BuildProjectDialog dialog = new BuildProjectDialog
            {
                Owner = window
            };
            dialog.ShowDialog();
        }
    }
}
