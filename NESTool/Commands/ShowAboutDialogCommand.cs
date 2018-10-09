using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class ShowAboutDialogCommand : Command
    {
        public override void Execute(object parameter)
        {
            var window = parameter as Window;

            AboutDialog dialog = new AboutDialog();
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
