using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class DeleteSelectedMapElement : Command
{
    public override bool CanExecute(object parameter)
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

    public override void Execute(object parameter)
    {
        if (parameter == null)
        {
            return;
        }

        int index = (int)parameter;

        SignalManager.Get<DeleteSelectedMapElementSignal>().Dispatch(index);
    }
}
