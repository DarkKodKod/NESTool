using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System.Windows.Input;

namespace NESTool.Commands
{
    public class PreviewMouseLeftButtonDownCommand : Command
    {
        public override void Execute(object parameter)
        {
            var mouseEvent = parameter as MouseButtonEventArgs;

            var position = mouseEvent.GetPosition(null);

            SignalManager.Get<MouseLeftButtonDownSignal>().Dispatch(position);
        }
    }
}
