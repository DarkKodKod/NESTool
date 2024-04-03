using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands;

public class OpenAddMapElementCommand : Command
{
    public override void Execute(object parameter)
    {
        Window parentWindow = Window.GetWindow((DependencyObject)parameter);

        AddMapElementDialog dialog = new();
        dialog.Owner = parentWindow;
        dialog.OnActivate();
        dialog.ShowDialog();
        dialog.OnDeactivate();
    }
}
