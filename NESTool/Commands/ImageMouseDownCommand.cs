using ArchitectureLibrary.Commands;
using System.Windows.Input;

namespace NESTool.Commands
{
    public class ImageMouseDownCommand : Command
    {
        public override void Execute(object parameter)
        {
            MouseButtonEventArgs mouseButtonEvent = parameter as MouseButtonEventArgs;
        }
    }
}
