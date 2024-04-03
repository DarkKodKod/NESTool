using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class SelectTileSetCommand : Command
{
    public override void Execute(object parameter)
    {
        string id = parameter as string;

        SignalManager.Get<SelectTileSetSignal>().Dispatch(id);
    }
}
