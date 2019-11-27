using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPalette
{
    public class PaletteEventArgs : RoutedEventArgs
    {
        internal PaletteEventArgs(object sender, Color c)
        {
            Sender = sender;
            C = c;
        }

        public object Sender { get; set; }
        public Color C { get; set; }
    }

    /// <summary>
    /// Interaction logic for ColorPaletteControl.xaml
    /// </summary>
    public partial class ColorPaletteControl : UserControl
    {
        public Color SelectedColor { get; set; }
        public Color CurrentColor { get; set; }
        public double PaletteHeight { get; set; } = 100;
        public double PaletteWidth { get; set; } = 360;
        public double CellHeight { get; set; } = 10;
        public double CellWidth { get; set; } = 10;

        public static readonly RoutedEvent ColorPaletteSelect = EventManager.RegisterRoutedEvent(
            "Select", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPaletteControl));
        public static readonly RoutedEvent ColorPaletteMouseMove = EventManager.RegisterRoutedEvent(
            "Hover", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPaletteControl));

        public event RoutedEventHandler Select
        {
            add { AddHandler(ColorPaletteSelect, value); }
            remove { RemoveHandler(ColorPaletteSelect, value); }
        }

        public event RoutedEventHandler Hover
        {
            add { AddHandler(ColorPaletteMouseMove, value); }
            remove { RemoveHandler(ColorPaletteMouseMove, value); }
        }

        public ColorPaletteControl()
        {
            InitializeComponent();
        }

        private void WrapPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorPaletteItem cpi = e.Source as ColorPaletteItem;

            SolidColorBrush scb = cpi.rect.Fill as SolidColorBrush;

            SelectedColor = scb.Color;

            PaletteEventArgs newEventArgs = new PaletteEventArgs(this, SelectedColor)
            {
                RoutedEvent = ColorPaletteSelect
            };
            RaiseEvent(newEventArgs);
        }

        private void WrapPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            ColorPaletteItem cpi = e.Source as ColorPaletteItem;

            SolidColorBrush scb = cpi.rect.Fill as SolidColorBrush;

            CurrentColor = scb.Color;

            PaletteEventArgs newEventArgs = new PaletteEventArgs(this, CurrentColor)
            {
                RoutedEvent = ColorPaletteMouseMove
            };
            RaiseEvent(newEventArgs);
        }

        private void WrapPanel1_MouseLeave(object sender, MouseEventArgs e)
        {
            PaletteEventArgs newEventArgs = new PaletteEventArgs(this, new Color())
            {
                RoutedEvent = ColorPaletteMouseMove
            };
            RaiseEvent(newEventArgs);
        }

        // Hardcoded NES palette
        public static readonly Color[] Colors = {
                Color.FromRgb(101,101,101), Color.FromRgb(0,45,105), Color.FromRgb(19,31,127), Color.FromRgb(60,19,124), Color.FromRgb(96,11,98),
                Color.FromRgb(115,10,55), Color.FromRgb(113,15,7), Color.FromRgb(90,26,0), Color.FromRgb(52,40,0), Color.FromRgb(11,52,0),
                Color.FromRgb(0,60,0), Color.FromRgb(0,61,16), Color.FromRgb(0,56,64), Color.FromRgb(0,0,0), Color.FromRgb(0,0,0),
                Color.FromRgb(0,0,0), Color.FromRgb(174,174,174), Color.FromRgb(15,99,179), Color.FromRgb(64,81,208), Color.FromRgb(120,65,204),
                Color.FromRgb(167,54,169), Color.FromRgb(192,52,112), Color.FromRgb(189,60,48), Color.FromRgb(159,74,0), Color.FromRgb(109,92,0),
                Color.FromRgb(54,109,0), Color.FromRgb(7,119,4), Color.FromRgb(0,121,61), Color.FromRgb(0,114,125), Color.FromRgb(0,0,0),
                Color.FromRgb(0,0,0), Color.FromRgb(0,0,0), Color.FromRgb(254,254,255), Color.FromRgb(93,179,255), Color.FromRgb(143,161,255),
                Color.FromRgb(200,144,255), Color.FromRgb(247,133,250), Color.FromRgb(255,131,192), Color.FromRgb(255,139,127), Color.FromRgb(239,154,73),
                Color.FromRgb(189,172,44), Color.FromRgb(133,188,47), Color.FromRgb(85,199,83), Color.FromRgb(60,201,140), Color.FromRgb(62,194,205),
                Color.FromRgb(78,78,78), Color.FromRgb(0,0,0), Color.FromRgb(0,0,0), Color.FromRgb(254,254,255), Color.FromRgb(188,223,255),
                Color.FromRgb(209,216,255), Color.FromRgb(232,209,255), Color.FromRgb(251,205,253), Color.FromRgb(255,204,229), Color.FromRgb(255,207,202),
                Color.FromRgb(248,213,180), Color.FromRgb(228,220,168), Color.FromRgb(204,227,169), Color.FromRgb(185,232,184), Color.FromRgb(174,232,208),
                Color.FromRgb(175,229,234), Color.FromRgb(182,182,182), Color.FromRgb(0,0,0), Color.FromRgb(0,0,0)
            };

        public static readonly string[] HexColors = {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0F", "0E", "0F",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F"
            };

        readonly ColorPaletteItem[] _paletteItems = {
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
            new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(), new ColorPaletteItem(),
        };

        private void ColorPalette_Loaded(object sender, RoutedEventArgs e)
        {
            WrapPanel1.Width = PaletteWidth;
            Border1.Width = PaletteWidth;
            WrapPanel1.Height = PaletteHeight;

            Border1.Height = 216 * (PaletteWidth % CellWidth) == 0 ? (216 * CellHeight / (PaletteWidth / CellWidth)) : (216 * CellHeight / PaletteWidth / CellWidth) + PaletteHeight;

            for (int i = 0; i < 64; i++)
            {
                _paletteItems[i].CellHeight = CellHeight;
                _paletteItems[i].CellWidth = CellWidth;
                _paletteItems[i].Red = Colors[i].R;
                _paletteItems[i].Green = Colors[i].G;
                _paletteItems[i].Blue = Colors[i].B;

                WrapPanel1.Children.Add(_paletteItems[i]);
            }
        }
    }
}
