using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using System.Windows.Media;

namespace NESTool.ViewModels
{
    public class PaletteViewModel : ItemViewModel
    {
		private bool _doNotSavePalettes = false;

        public override void OnActivate()
        {
            base.OnActivate();

			#region Signals
			SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
			#endregion

			LoadPalettes();
		}

        public override void OnDeactivate()
        {
            base.OnDeactivate();

			#region Signals
			SignalManager.Get<ColorPaletteControlSelectedSignal>().RemoveListener(OnColorPaletteControlSelected);
			#endregion
		}

		public PaletteModel GetModel()
		{
			if (ProjectItem?.FileHandler.FileModel is PaletteModel model)
			{
				return model;
			}

			return null;
		}

		private void LoadPalettes()
		{
			_doNotSavePalettes = true;

			byte R;
			byte G;
			byte B;

			// Load palettes
			for (int i = 0; i < 4; ++i)
			{
				int color0 = GetModel().Color0;
				R = (byte)(color0 >> 16);
				G = (byte)(color0 >> 8);
				B = (byte)color0;

				SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 0);

				int color1 = GetModel().Color1;
				R = (byte)(color1 >> 16);
				G = (byte)(color1 >> 8);
				B = (byte)color1;

				SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 1);

				int color2 = GetModel().Color2;
				R = (byte)(color2 >> 16);
				G = (byte)(color2 >> 8);
				B = (byte)color2;

				SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 2);

				int color3 = GetModel().Color3;
				R = (byte)(color3 >> 16);
				G = (byte)(color3 >> 8);
				B = (byte)color3;

				SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 3);
			}

			_doNotSavePalettes = false;
		}

		private void OnColorPaletteControlSelected(Color color, int paletteIndex, int colorPosition)
		{
			if (!IsActive)
			{
				return;
			}

			if (_doNotSavePalettes)
			{
				return;
			}

			int colorInt = ((color.R & 0xff) << 16) | ((color.G & 0xff) << 8) | (color.B & 0xff);

            Palette palette = new Palette
            {
                Color0 = GetModel().Color0,
                Color1 = GetModel().Color1,
                Color2 = GetModel().Color2,
                Color3 = GetModel().Color3
            };

            switch (colorPosition)
			{
				case 0: palette.Color0 = colorInt; break;
				case 1: palette.Color1 = colorInt; break;
				case 2: palette.Color2 = colorInt; break;
				case 3: palette.Color3 = colorInt; break;
			}

            GetModel().Palette = palette;

            ProjectItem.FileHandler.Save();
		}
	}
}
