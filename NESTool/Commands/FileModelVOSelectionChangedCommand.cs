using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows.Controls;

namespace NESTool.Commands;

public class FileModelVOSelectionChangedCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        SelectionChangedEventArgs? changedEvent = parameter as SelectionChangedEventArgs;

        if (changedEvent?.AddedItems.Count > 0)
        {
            if (changedEvent.AddedItems[0] is FileModelVO fileModel)
            {
                SignalManager.Get<FileModelVOSelectionChangedSignal>().Dispatch(fileModel);
            }
        }
    }
}
