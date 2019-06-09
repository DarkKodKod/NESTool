using System.ComponentModel;
using System.Windows.Controls;
using NESTool.Commands;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for PaletteView.xaml
    /// </summary>
    public partial class PaletteView : UserControl, INotifyPropertyChanged
    {
        private int _paletteIndex;

        #region Commands
        public ShowColorPaletteCommand ShowColorPaletteCommand { get; } = new ShowColorPaletteCommand();
        #endregion

        public int PaletteIndex
        {
            get { return _paletteIndex; }
            set
            {
                _paletteIndex = value;

                OnPropertyChanged("PaletteIndex");
            }
        }

        public PaletteView()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }
    }
}
