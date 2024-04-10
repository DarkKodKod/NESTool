using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using System.Windows.Media;

namespace NESTool.ViewModels;

public class PaletteViewModel : ItemViewModel
{
    private bool _doNotSavePalettes = false;

    public override void OnActivate()
    {
        base.OnActivate();

        #region Signals
        SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener += OnColorPaletteControlSelected;
        #endregion

        LoadPalettes();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        #region Signals
        SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener -= OnColorPaletteControlSelected;
        #endregion
    }

    public PaletteModel? GetModel()
    {
        if (ProjectItem?.FileHandler.FileModel is PaletteModel model)
        {
            return model;
        }

        return null;
    }

    private void LoadPalettes()
    {
        PaletteModel? model = GetModel();

        if (model == null)
        {
            return;
        }

        _doNotSavePalettes = true;

        // Load palettes
        for (int i = 0; i < 4; ++i)
        {
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(model.Color0), (PaletteIndex)i, 0);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(model.Color1), (PaletteIndex)i, 1);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(model.Color2), (PaletteIndex)i, 2);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(model.Color3), (PaletteIndex)i, 3);
        }

        _doNotSavePalettes = false;
    }

    private void OnColorPaletteControlSelected(Color color, PaletteIndex paletteIndex, int colorPosition)
    {
        if (!IsActive)
        {
            return;
        }

        if (_doNotSavePalettes)
        {
            return;
        }

        PaletteModel? model = GetModel();

        if (model == null)
        {
            return;
        }

        int colorInt = ((color.R & 0xff) << 16) | ((color.G & 0xff) << 8) | (color.B & 0xff);

        int color0 = model.Color0;
        int color1 = model.Color1;
        int color2 = model.Color2;
        int color3 = model.Color3;

        switch (colorPosition)
        {
            case 0: color0 = colorInt; break;
            case 1: color1 = colorInt; break;
            case 2: color2 = colorInt; break;
            case 3: color3 = colorInt; break;
        }

        model.Color0 = color0;
        model.Color1 = color1;
        model.Color2 = color2;
        model.Color3 = color3;

        ProjectItem?.FileHandler.Save();
    }
}
