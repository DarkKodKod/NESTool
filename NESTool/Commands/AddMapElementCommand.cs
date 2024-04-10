using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class AddMapElementCommand : Command
{
    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        string mapElement = (string)parameter;

        if (string.IsNullOrEmpty(mapElement))
        {
            return false;
        }

        return true;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        string mapElementId = (string)parameter;

        SignalManager.Get<AddMapElementSignal>().Dispatch(mapElementId);
    }
}
