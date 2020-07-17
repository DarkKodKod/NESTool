using NESTool.UserControls;
using System.Windows.Controls;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for World.xaml
    /// </summary>
    public partial class World : UserControl, ICleanable
    {
        public World()
        {
            InitializeComponent();
        }

        public void CleanUp()
        {
            //
        }
    }
}
