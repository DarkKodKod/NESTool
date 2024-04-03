using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Signals;
using System.Windows.Controls;

namespace NESTool.Commands;

public class SourceSelectionChangedCommand : Command
{
    public override void Execute(object parameter)
    {
        SelectionChangedEventArgs changedEvent = parameter as SelectionChangedEventArgs;

        if (changedEvent.AddedItems.Count > 0)
        {
            if (changedEvent.AddedItems[0] is EntitySource entitySource)
            {
                SignalManager.Get<EntitySourceSelectionChangedSignal>().Dispatch(entitySource);
            }
        }
    }
}
