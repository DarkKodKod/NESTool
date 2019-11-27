using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Palette.xaml
    /// </summary>
    public partial class Palette : UserControl, ICleanable
    {
        public Palette()
        {
            InitializeComponent();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
            #endregion
        }

        private void OnColorPaletteControlSelected(Color color, int paletteIndex, int colorPosition)
        {
            if (DataContext is PaletteViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            SolidColorBrush scb = new SolidColorBrush
            {
                Color = color
            };

            switch (colorPosition)
            {
                case 0: palette.cvsColor0.Background = scb; break;
                case 1: palette.cvsColor1.Background = scb; break;
                case 2: palette.cvsColor2.Background = scb; break;
                case 3: palette.cvsColor3.Background = scb; break;
            }
        }

        private void ColorPaletteCleanup()
        {
            Color color = Color.FromRgb(0, 0, 0);
            SolidColorBrush brush = new SolidColorBrush(color);

            palette.cvsColor0.Background = brush;
            palette.cvsColor1.Background = brush;
            palette.cvsColor2.Background = brush;
            palette.cvsColor3.Background = brush;
        }

        public void CleanUp()
        {
            ColorPaletteCleanup();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().RemoveListener(OnColorPaletteControlSelected);
            #endregion
        }
    }
}
