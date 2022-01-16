using ArchitectureLibrary.Signals;
using ColorPalette;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.ViewModels;
using NESTool.VOs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for TileSet.xaml
    /// </summary>
    public partial class TileSet : UserControl, ICleanable
    {
        private Color _selectedColor = new Color();
        private bool _mouseDown = false;

        public TileSet()
        {
            InitializeComponent();

            SignalManager.Get<MouseWheelSignal>().Listener += OnMouseWheel;
            SignalManager.Get<ColorPaletteSelectSignal>().Listener += OnColorPaletteSelect;
        }

        private void OnColorPaletteSelect(Color color)
        {
            if (DataContext is TileSetViewModel viewModel)
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

            _selectedColor = color;

            mask.Background = scb;
        }

        private void OnMouseWheel(MouseWheelVO vo)
        {
            if (DataContext is TileSetViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            const double ScaleRate = 1.1;

            if (vo.Delta > 0)
            {
                scaleCanvas.ScaleX *= ScaleRate;
                scaleCanvas.ScaleY *= ScaleRate;
            }
            else
            {
                scaleCanvas.ScaleX /= ScaleRate;
                scaleCanvas.ScaleY /= ScaleRate;
            }
        }

        private void ColorPalette1_Hover(object sender, RoutedEventArgs e)
        {
            if (e is PaletteEventArgs args)
            {
                if (args.C == new Color())
                {
                    if (_selectedColor != new Color())
                    {
                        SolidColorBrush scb = new SolidColorBrush
                        {
                            Color = _selectedColor
                        };

                        mask.Background = scb;
                    }
                    else
                    {
                        mask.Background = Brushes.DarkGray;
                    }
                }
                else
                {
                    SolidColorBrush scb = new SolidColorBrush
                    {
                        Color = args.C
                    };
                    mask.Background = scb;
                }
            }
        }

        public void CleanUp()
        {
            SignalManager.Get<MouseWheelSignal>().Listener -= OnMouseWheel;
            SignalManager.Get<ColorPaletteSelectSignal>().Listener -= OnColorPaletteSelect;
        }

        private void ImgSmall_MouseUp(object sender, MouseButtonEventArgs e)
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

        private void ImgSmall_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void ImgSmall_MouseLeave(object sender, MouseEventArgs e)
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

        private void ImgSmall_MouseMove(object sender, MouseEventArgs e)
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

                int x = (int)Math.Floor(point.X / 8);
                int y = (int)Math.Floor(point.Y / 8);

                if (x < 0 || y < 0 || x >= 8 || y >= 8)
                {
                    return;
                }

                WriteableBitmap writeableBmp = BitmapFactory.ConvertToPbgra32Format(image.Source as BitmapSource);

                SignalManager.Get<SelectedPixelSignal>().Dispatch(writeableBmp.Clone(), new Point(x, y));
            }
        }
    }
}
