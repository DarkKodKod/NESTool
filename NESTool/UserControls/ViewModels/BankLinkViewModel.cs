using ArchitectureLibrary.ViewModel;
using NESTool.Commands;

namespace NESTool.UserControls.ViewModels
{
    public class BankLinkViewModel : ViewModel
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

        #region Commands
        public SelectTileSetCommand SelectTileSetCommand { get; } = new SelectTileSetCommand();
        #endregion
    }
}
