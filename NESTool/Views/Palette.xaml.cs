using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.Utils;
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

        private void OnColorPaletteControlSelected(Color color, PaletteIndex paletteIndex, int colorPosition)
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
                case 0: 
                    palette.cvsColor0.Background = scb;
                    palette.hexColor0.Text = Util.ColorToColorHex(color);
                    break;
                case 1: 
                    palette.cvsColor1.Background = scb;
                    palette.hexColor1.Text = Util.ColorToColorHex(color);
                    break;
                case 2: 
                    palette.cvsColor2.Background = scb;
                    palette.hexColor2.Text = Util.ColorToColorHex(color);
                    break;
                case 3: 
                    palette.cvsColor3.Background = scb;
                    palette.hexColor3.Text = Util.ColorToColorHex(color);
                    break;
            }
        }

        private void ColorPaletteCleanup()
        {
            Color color = Color.FromRgb(0, 0, 0);
            SolidColorBrush brush = new SolidColorBrush(color);

            palette.cvsColor0.Background = brush;
            palette.hexColor0.Text = "0F";
            palette.cvsColor1.Background = brush;
            palette.hexColor1.Text = "0F";
            palette.cvsColor2.Background = brush;
            palette.hexColor2.Text = "0F";
            palette.cvsColor3.Background = brush;
            palette.hexColor3.Text = "0F";
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
