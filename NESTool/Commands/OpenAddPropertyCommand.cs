using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class OpenAddPropertyCommand : Command
    {
        public override void Execute(object parameter)
        {
            Window parentWindow = Window.GetWindow((DependencyObject)parameter);

            AddPropertyDialog dialog = new AddPropertyDialog();
            dialog.Owner = parentWindow;
            dialog.OnActivate();
            dialog.ShowDialog();
            dialog.OnDeactivate();
        }
    }
}
