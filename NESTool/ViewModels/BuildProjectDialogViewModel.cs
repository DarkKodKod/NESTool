using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.ViewModels
{
    public class BuildProjectDialogViewModel : ViewModel
    {
        private string _folderPath;

        #region Commands
        public BuildProjectCommand BuildProjectCommand { get; } = new BuildProjectCommand();
        public BrowseFolderCommand BrowseFolderCommand { get; } = new BrowseFolderCommand();
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
        #endregion

        #region get/set
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }
        #endregion

        public BuildProjectDialogViewModel()
        {
            SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);

            ProjectModel project = ModelManager.Get<ProjectModel>();

            FolderPath = project.OutputFilePath;
        }

        private void OnCloseDialog()
        {
            // Save changes
            ProjectModel project = ModelManager.Get<ProjectModel>();

            if (project.OutputFilePath != FolderPath)
            {
                project.OutputFilePath = FolderPath;

                project.Save();
            }

            SignalManager.Get<BrowseFolderSuccessSignal>().RemoveListener(BrowseFolderSuccess);
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        private void BrowseFolderSuccess(string folderPath) => FolderPath = folderPath;
    }
}
