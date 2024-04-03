using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class AddPropertyCommand : Command
{
    public override bool CanExecute(object parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        string property = (string)parameter;

        if (string.IsNullOrEmpty(property))
        {
            return false;
        }

        return true;
    }

    public override void Execute(object parameter)
    {
        string property = (string)parameter;

        SignalManager.Get<AddPropertySignal>().Dispatch(property);
    }
}
