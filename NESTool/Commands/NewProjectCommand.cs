using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands;

public class NewProjectCommand : Command
{
    public override void Execute(object parameter)
    {
        Window window = parameter as Window;

        ProjectDialog dialog = new();
        dialog.Owner = window;
        dialog.ShowDialog();
    }
}
