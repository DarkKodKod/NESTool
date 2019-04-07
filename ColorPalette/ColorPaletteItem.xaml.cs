using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPalette
{
    /// <summary>
    /// Interaction logic for ColorPaletteItem.xaml
    /// </summary>
    public partial class ColorPaletteItem : UserControl
    {
        public byte Red { get; set; }
        public byte Blue { get; set; }
        public byte Green { get; set; }
        public double CellHeight { get; set; } = 10;
        public double CellWidth { get; set; } = 10;

        public ColorPaletteItem()
        {
            InitializeComponent();
        }

        private void ColorPaletteItem_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SolidColorBrush scb = new SolidColorBrush();

            Color col = new Color
            {
                A = 255,
                R = Red,
                B = Blue,
                G = Green
            };

            scb.Color = col;

            rect.Height = CellHeight;
            rect.Width = CellWidth;
            rect.Fill = scb;
        }
    }
}
