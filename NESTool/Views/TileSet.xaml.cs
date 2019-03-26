using ColorPalette;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for TileSet.xaml
    /// </summary>
    public partial class TileSet : UserControl
    {
        public TileSet()
        {
            InitializeComponent();
        }

        private void ColorPalette1_Hover(object sender, RoutedEventArgs e)
        {
            if (e is PaletteEventArgs args)
            {
                if (args.C == new Color())
                {
                    Canvas1.Background = Brushes.DarkGray;

                    SolidColorBrush scb = Canvas1.Background as SolidColorBrush;
                }
                else
                {
                    SolidColorBrush scb = new SolidColorBrush();
                    scb.Color = args.C;
                    Canvas1.Background = scb;
                }
            }
        }
    }
}
