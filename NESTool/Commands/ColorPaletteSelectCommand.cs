using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using ColorPalette;
using NESTool.Signals;

namespace NESTool.Commands;

public class ColorPaletteSelectCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        PaletteEventArgs? palette = parameter as PaletteEventArgs;

        if (palette != null)
            SignalManager.Get<ColorPaletteSelectSignal>().Dispatch(palette.C);
    }
}
