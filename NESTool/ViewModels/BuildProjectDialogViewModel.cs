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
        private bool _useRLEOnMaps = false;

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

        public bool UseRLEOnMaps
        {
            get { return _useRLEOnMaps; }
            set
            {
                if (_useRLEOnMaps != value)
                {
                    ProjectModel project = ModelManager.Get<ProjectModel>();
                    project.Build.UseRLEOnMaps = value;
                    project.Save();

                    _useRLEOnMaps = value;

                    OnPropertyChanged("UseRLEOnMaps");
                }
            }
        }
        #endregion

        public BuildProjectDialogViewModel()
        {
            #region Signals
            SignalManager.Get<BrowseFolderSuccessSignal>().Listener += BrowseFolderSuccess;
            SignalManager.Get<CloseDialogSignal>().Listener += OnCloseDialog;
            #endregion

            ProjectModel project = ModelManager.Get<ProjectModel>();

            FolderPath = project.Build.OutputFilePath;
            UseRLEOnMaps = project.Build.UseRLEOnMaps;
        }

        private void OnCloseDialog()
        {
            #region Signals
            SignalManager.Get<BrowseFolderSuccessSignal>().Listener -= BrowseFolderSuccess;
            SignalManager.Get<CloseDialogSignal>().Listener -= OnCloseDialog;
            #endregion
        }

        private void BrowseFolderSuccess(string folderPath)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            if (project.Build.OutputFilePath != folderPath)
            {
                FolderPath = folderPath;

                project.Build.OutputFilePath = folderPath;

                project.Save();
            }
        }
    }
}
