using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class MoveDownSelectedMapElement : Command
{
    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        if ((int)parameter == -1)
        {
            return false;
        }

        return true;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        int selectedPropertyIndex = (int)parameter;

        SignalManager.Get<MoveDownSelectedMapElementSignal>().Dispatch(selectedPropertyIndex);
    }
}
