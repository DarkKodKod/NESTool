using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.UserControls.Views;
using NESTool.Utils;
using NESTool.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, ICleanable
    {
        private bool _mouseDown = false;

        public Map()
        {
            InitializeComponent();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener += OnColorPaletteControlSelected;
            #endregion
        }

        private void OnColorPaletteControlSelected(Color color, PaletteIndex paletteIndex, int colorPosition)
        {
            if (DataContext is MapViewModel viewModel)
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

            PaletteView palette = null;

            switch (paletteIndex)
            {
                case PaletteIndex.Palette0: palette = palette0; break;
                case PaletteIndex.Palette1: palette = palette1; break;
                case PaletteIndex.Palette2: palette = palette2; break;
                case PaletteIndex.Palette3: palette = palette3; break;
            }

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

            void SetColorBack(PaletteView palette)
            {
                palette.cvsColor0.Background = brush;
                palette.cvsColor1.Background = brush;
                palette.cvsColor2.Background = brush;
                palette.cvsColor3.Background = brush;
            }

            SetColorBack(palette0);
            SetColorBack(palette1);
            SetColorBack(palette2);
            SetColorBack(palette3);
        }

        public void CleanUp()
        {
            ColorPaletteCleanup();

            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener -= OnColorPaletteControlSelected;
        }

        private void ImgFrame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                if (image.IsMouseCaptured)
                {
                    image.ReleaseMouseCapture();
                }
            }

            _mouseDown = false;
        }

        private void ImgFrame_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                if (image.IsMouseCaptured)
                {
                    image.ReleaseMouseCapture();
                }
            }

            _mouseDown = false;
        }

        private void ImgFrame_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
            {
                return;
            }

            if (sender is Image image)
            {
                if (!image.IsMouseCaptured)
                {
                    return;
                }

                Point point = e.GetPosition(image);

                Util.SendSelectedQuadrantSignal(image, point);
            }
        }

        private void ImgFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                if (!image.IsMouseCaptured)
                {
                    image.CaptureMouse();
                }
            }

            _mouseDown = true;
        }
    }
}
