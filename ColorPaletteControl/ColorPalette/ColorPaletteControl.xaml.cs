using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

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

            PaletteEventArgs newEventArgs = new PaletteEventArgs(this, SelectedColor);
            newEventArgs.RoutedEvent = ColorPaletteSelect;
            RaiseEvent(newEventArgs);
        }

        private void WrapPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            ColorPaletteItem cpi = e.Source as ColorPaletteItem;

            SolidColorBrush scb = cpi.rect.Fill as SolidColorBrush;

            CurrentColor = scb.Color;

            PaletteEventArgs newEventArgs = new PaletteEventArgs(this, CurrentColor);
            newEventArgs.RoutedEvent = ColorPaletteMouseMove;
            RaiseEvent(newEventArgs);
        }

        private void WrapPanel1_MouseLeave(object sender, MouseEventArgs e)
        {
            PaletteEventArgs newEventArgs = new PaletteEventArgs(this, new Color());
            newEventArgs.RoutedEvent = ColorPaletteMouseMove;
            RaiseEvent(newEventArgs);
        }

        private void ColorPalette_Loaded(object sender, RoutedEventArgs e)
        {
            WrapPanel1.Width = PaletteWidth;
            Border1.Width = PaletteWidth;
            WrapPanel1.Height = PaletteHeight;

            Border1.Height = 216 * (PaletteWidth % CellWidth) == 0 ? (216 * CellHeight / (PaletteWidth / CellWidth)) : (216 * CellHeight / PaletteWidth / CellWidth) + PaletteHeight;
            
            for (int i = 0; i < 255; i = i + 50)
            {
                for (int j = 0; j < 255; j = j + 50)
                {
                    for (int k = 0; k < 255; k = k + 50)
                    {
                        ColorPaletteItem cpi = new ColorPaletteItem();
                        cpi.CellHeight = CellHeight;
                        cpi.CellWidth = CellWidth;
                        cpi.Red = (byte)i;
                        cpi.Green = (byte)j;
                        cpi.Blue = (byte)k;

                        WrapPanel1.Children.Add(cpi);
                    }
                }
            }
        }
    }
}
