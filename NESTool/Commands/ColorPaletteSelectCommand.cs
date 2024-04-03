using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using ColorPalette;
using NESTool.Signals;

namespace NESTool.Commands;

public class ColorPaletteSelectCommand : Command
{
    public override void Execute(object parameter)
    {
        PaletteEventArgs palette = parameter as PaletteEventArgs;

        SignalManager.Get<ColorPaletteSelectSignal>().Dispatch(palette.C);
    }
}
