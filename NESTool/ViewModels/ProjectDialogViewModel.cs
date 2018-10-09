using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Signals;
using NESTool.Utils;

namespace NESTool.ViewModels
{
    public class ProjectDialogViewModel : ViewModel
    {
        public CreateProjectCommand CreateProjectCommand { get; } = new CreateProjectCommand();
        public BrowseFolderCommand BrowseFolderCommand { get; } = new BrowseFolderCommand();

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

        private string _previousValidName;
        private string _projectName;
        private string _folderPath;

        public ProjectDialogViewModel()
        {
            SignalManager.Get<CreateProjectSuccessSignal>().AddListener(CreateProjectSuccess);
            SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);
        }

        ~ProjectDialogViewModel()
        {
            SignalManager.Get<CreateProjectSuccessSignal>().RemoveListener(CreateProjectSuccess);
            SignalManager.Get<BrowseFolderSuccessSignal>().RemoveListener(BrowseFolderSuccess);
        }
        
        private void BrowseFolderSuccess(string folderPath) => FolderPath = folderPath;

        private void CreateProjectSuccess(string projectFullPath)
        {
            // After creating the project now it is time to open it
            var openProjectCommand = new OpenProjectCommand();
            if (openProjectCommand.CanExecute(projectFullPath))
            {
                openProjectCommand.Execute(projectFullPath);
            }
        }
    }
}
