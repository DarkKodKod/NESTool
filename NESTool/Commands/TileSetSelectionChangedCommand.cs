using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class TileSetSelectionChangedCommand : Command
    {
        public override void Execute(object parameter)
        {
            SelectionChangedEventArgs changedEvent = parameter as SelectionChangedEventArgs;

            if (changedEvent.AddedItems.Count > 0)
            {
                if (changedEvent.AddedItems[0] is FileModelVO fileModel)
                {
                    SignalManager.Get<TileSetSelectionChangedSignal>().Dispatch(fileModel);
                }
            }
        }
    }
}
