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
        public ClosedNewProjectCommand ClosedNewProjectCommand { get; } = new ClosedNewProjectCommand();

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

        public int CHRSize
        {
            get { return _chrSize; }
            set
            {
                _chrSize = value;
                OnPropertyChanged("CHRSize");
            }
        }

        public int PRGSize
        {
            get { return _prgSize; }
            set
            {
                _prgSize = value;
                OnPropertyChanged("PRGSize");
            }
        }
        #endregion

        private string _previousValidName;
        private string _projectName;
        private string _folderPath;
        private int _chrSize = 0;
        private int _prgSize = 0;
        private MapperModel[] _mappers;
        private int _selectedMapper;

        public ProjectDialogViewModel()
        {
            var mappers = ModelManager.Get<MappersModel>();

            Mappers = mappers.Mappers;
            SelectedMapper = mappers.Mappers[0].Id;

            SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);
            SignalManager.Get<ClosedNewProjectSignal>().AddListener(OnClosedNewProject);
        }

        private void OnClosedNewProject()
        {
            SignalManager.Get<BrowseFolderSuccessSignal>().RemoveListener(BrowseFolderSuccess);
            SignalManager.Get<ClosedNewProjectSignal>().RemoveListener(OnClosedNewProject);
        }

        private void BrowseFolderSuccess(string folderPath) => FolderPath = folderPath;
    }
}
