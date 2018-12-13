using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;

namespace NESTool.ViewModels
{
    public class ProjectDialogViewModel : ViewModel
    {
        public CreateProjectCommand CreateProjectCommand { get; } = new CreateProjectCommand();
        public BrowseFolderCommand BrowseFolderCommand { get; } = new BrowseFolderCommand();
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();

        #region get/set
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                if (Util.ValidFileName(value))
                {
                    _projectName = value;
                    _previousValidName = value;
                    OnPropertyChanged("ProjectName");
                }
                else
                {
                    ProjectName = _previousValidName;
                }
            }
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        public MapperModel[] Mappers
        {
            get { return _mappers; }
            set
            {
                _mappers = value;
                OnPropertyChanged("Mappers");
            }
        }

        public int SelectedMapper
        {
            get { return _selectedMapper; }
            set
            {
                _selectedMapper = value;
                OnPropertyChanged("SelectedMapper");
            }
        }

        public int[] CHRSizes
        {
            get { return _chrSizes; }
            set
            {
                _chrSizes = value;
                OnPropertyChanged("CHRSizes");
            }
        }

        public int SelectedCHRSize
        {
            get { return _selectedCHRSize; }
            set
            {
                _selectedCHRSize = value;
                OnPropertyChanged("SelectedCHRSize");
            }
        }

        public int[] PRGSizes
        {
            get { return _prgSizes; }
            set
            {
                _prgSizes = value;
                OnPropertyChanged("PRGSizes");
            }
        }

        public int SelectedPRGSize
        {
            get { return _selectedPRGSize; }
            set
            {
                _selectedPRGSize = value;
                OnPropertyChanged("SelectedPRGSize");
            }
        }
        #endregion

        private string _previousValidName;
        private string _projectName;
        private string _folderPath;
        private int[] _chrSizes;
        private int[] _prgSizes;
        private MapperModel[] _mappers;
        private int _selectedMapper;
        private int _selectedCHRSize;
        private int _selectedPRGSize;

        public ProjectDialogViewModel()
        {
            var mappers = ModelManager.Get<MappersModel>();

            Mappers = mappers.Mappers;
            SelectedMapper = mappers.Mappers[0].Id;
            CHRSizes = mappers.Mappers[0].CHR;
            SelectedCHRSize = 0;
            PRGSizes = mappers.Mappers[0].PRG;
            SelectedPRGSize = 0;

            SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);
        }

        private void OnCloseDialog()
        {
            SignalManager.Get<BrowseFolderSuccessSignal>().RemoveListener(BrowseFolderSuccess);
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        private void BrowseFolderSuccess(string folderPath) => FolderPath = folderPath;
    }
}
