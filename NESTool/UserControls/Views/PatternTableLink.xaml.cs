using NESTool.Commands;
using System.ComponentModel;
using System.Windows.Controls;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for PatternTableLink.xaml
    /// </summary>
    public partial class PatternTableLink : UserControl, INotifyPropertyChanged
    {
        private string _caption;
        private string _tileSetId;

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;

                OnPropertyChanged("Caption");
            }
        }

        public string TileSetId
        {
            get { return _tileSetId; }
            set
            {
                _tileSetId = value;

                OnPropertyChanged("TileSetId");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        #region Commands
        public SelectTileSetCommand SelectTileSetCommand { get; } = new SelectTileSetCommand();
        #endregion

        public PatternTableLink(string caption, string id)
        {
            InitializeComponent();

            TileSetId = id;
            Caption = caption;
        }
    }
}
