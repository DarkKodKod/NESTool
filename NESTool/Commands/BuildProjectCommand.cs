using ArchitectureLibrary.Commands;
using System.Windows;

namespace NESTool.Commands
{
    public class BuildProjectCommand : Command
    {
        public override void Execute(object parameter)
        {
            MessageBox.Show("Build completed!", "Build", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
