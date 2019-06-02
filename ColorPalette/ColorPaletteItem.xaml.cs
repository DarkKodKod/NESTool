using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPalette
{
    /// <summary>
    /// Interaction logic for ColorPaletteItem.xaml
    /// </summary>
    public partial class ColorPaletteItem : UserControl
    {
        private readonly SolidColorBrush _solidColor = new SolidColorBrush();

        public byte Red { get; set; }
        public byte Blue { get; set; }
        public byte Green { get; set; }
        public double CellHeight { get; set; } = 10;
        public double CellWidth { get; set; } = 10;

        public ColorPaletteItem()
        {
            InitializeComponent();
        }

        private void ColorPaletteItem_Loaded(object sender, RoutedEventArgs e)
        {
            Color col = new Color
            {
                A = 255,
                R = Red,
                B = Blue,
                G = Green
            };

            _solidColor.Color = col;
            _solidColor.Freeze();

            rect.Height = CellHeight;
            rect.Width = CellWidth;
            rect.Fill = _solidColor;
        }
    }
}
