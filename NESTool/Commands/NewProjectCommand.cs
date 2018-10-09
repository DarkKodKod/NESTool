using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class NewProjectCommand : Command
    {
        public override void Execute(object parameter)
        {
            var window = parameter as Window;

            ProjectDialog dialog = new ProjectDialog();
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
