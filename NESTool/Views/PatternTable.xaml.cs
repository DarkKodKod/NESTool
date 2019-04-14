using ArchitectureLibrary.Signals;
using NESTool.Signals;
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
