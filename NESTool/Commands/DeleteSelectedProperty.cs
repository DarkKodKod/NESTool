using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class DeleteSelectedProperty : Command
{
    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        string selectedProperty = (string)parameter;

        if (string.IsNullOrEmpty(selectedProperty))
        {
            return false;
        }

        return true;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        string selectedProperty = (string)parameter;

        SignalManager.Get<DeleteSelectedPropertySignal>().Dispatch(selectedProperty);
    }
}
