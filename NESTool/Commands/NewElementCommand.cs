using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.Models;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class NewElementCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            ProjectModel model = ModelManager.Get<ProjectModel>();

            if (model != null && !string.IsNullOrEmpty(model.Name))
            {
                return true;
            }

            return false;
        }

        public override void Execute(object parameter)
        {
            var window = parameter as Window;

            ElementDialog dialog = new ElementDialog();
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
