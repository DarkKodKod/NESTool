using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands;

public class SelectTileSetCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        string? id = parameter as string;

        if (id == null)
            return;

        SignalManager.Get<SelectTileSetSignal>().Dispatch(id);
    }
}
