using ArchitectureLibrary.ViewModel;
using NESTool.Commands;

namespace NESTool.UserControls.ViewModels;

public class BankLinkViewModel : ViewModel
{
    private string _caption = string.Empty;
    private string _tileSetId = string.Empty;

    public string Caption
    {
        get => _caption;
        set
        {
            _caption = value;

            OnPropertyChanged("Caption");
        }
    }

    public string TileSetId
    {
        get => _tileSetId;
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
