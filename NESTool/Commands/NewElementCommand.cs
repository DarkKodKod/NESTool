using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class NewElementCommand : Command
    {
        public override void Execute(object parameter)
        {
            var window = parameter as Window;

            ElementDialog dialog = new ElementDialog();
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
