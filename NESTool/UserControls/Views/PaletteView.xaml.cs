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
        #region Commands
        public ShowColorPaletteCommand ShowColorPaletteCommand { get; } = new ShowColorPaletteCommand();
        #endregion

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
