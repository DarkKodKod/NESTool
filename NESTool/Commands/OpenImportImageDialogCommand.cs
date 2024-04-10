using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.Models;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands;

public class OpenImportImageDialogCommand : Command
{
    public override bool CanExecute(object? parameter)
    {
        ProjectModel model = ModelManager.Get<ProjectModel>();

        if (model != null && !string.IsNullOrEmpty(model.Name))
        {
            return true;
        }

        return false;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        Window? window = parameter as Window;

        ImportImageDialog dialog = new()
        {
            Owner = window
        };
        dialog.ShowDialog();
    }
}
