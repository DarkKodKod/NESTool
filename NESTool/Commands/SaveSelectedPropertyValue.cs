using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class SaveSelectedPropertyValue : Command
{
    public override bool CanExecute(object parameter)
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

    public override void Execute(object parameter)
    {
        string selectedPropertyValue = (string)parameter;

        SignalManager.Get<SaveSelectedPropertyValueSignal>().Dispatch(selectedPropertyValue);
    }
}
