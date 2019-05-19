using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.UserControls.Views;
using NESTool.VOs;
using System.Windows.Controls;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for PatternTable.xaml
    /// </summary>
    public partial class PatternTable : UserControl
    {
        public PatternTable()
        {
            InitializeComponent();

            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<AddNewTileSetLinkSignal>().AddListener(OnAddNewTileSetLink);
            SignalManager.Get<CleanupTileSetLinksSignal>().AddListener(OnCleanupTileSetLinks);
        }

        private void OnCleanupTileSetLinks()
        {
            wpLinks.Children.Clear();
        }

        private void OnAddNewTileSetLink(PatternTableLinkVO vo)
        {
            wpLinks.Children.Add(new PatternTableLink(vo.Caption, vo.Id));
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
    }
}
