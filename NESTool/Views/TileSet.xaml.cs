﻿using ArchitectureLibrary.Signals;
using ColorPalette;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for TileSet.xaml
    /// </summary>
    public partial class TileSet : UserControl
    {
        private Color _selectedColor = new Color();

        public TileSet()
        {
            InitializeComponent();

            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
        }

        private void OnMouseWheel(MouseWheelVO vo)
        {
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

        private void ColorPalette1_Select(object sender, RoutedEventArgs e)
        {
            if (e is PaletteEventArgs args)
            {
                SolidColorBrush scb = new SolidColorBrush
                {
                    Color = args.C
                };

                _selectedColor = args.C;

                mask.Background = scb;
            }
        }
    }
}
