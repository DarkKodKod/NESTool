using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;

namespace NESTool.Commands
{
    public class OpenImportImageDialogCommand : Command
    {
        public override void Execute(object parameter)
        {
            Window window = parameter as Window;

            ImportImageDialog dialog = new ImportImageDialog();
            dialog.Owner = window;
            dialog.ShowDialog();
        }
    }
}
