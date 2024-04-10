using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands;

public class ShowAboutDialogCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        Window? window = parameter as Window;

        AboutDialog dialog = new();
        dialog.Owner = window;
        dialog.ShowDialog();
    }
}
