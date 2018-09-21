using NESTool.Architecture.Model;
using NESTool.Architecture.Signals;
using NESTool.Architecture.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NESTool.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public OpenProjectCommand OpenProjectCommand { get; } = new OpenProjectCommand();
        public CloseProjectCommand CloseProjectCommand { get; } = new CloseProjectCommand();
        public ExitCommand ExitCommand { get; } = new ExitCommand();
        public NewFileCommand NewFileCommand { get; } = new NewFileCommand();
        public NewProjectCommand NewProjectCommand { get; } = new NewProjectCommand();
        public SaveAllCommand SaveAllCommand { get; } = new SaveAllCommand();
        public SaveCommand SaveCommand { get; } = new SaveCommand();
        public LoadConfigsCommand LoadConfigsCommand { get; } = new LoadConfigsCommand();

        private List<ProjectItem> _projectItems;
        private string _title;
        
        public List<ProjectItem> ProjectItems
        {
            get { return _projectItems; }
            set
            {
                _projectItems = value;
                OnPropertyChanged("ProjectItems");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public MainWindowViewModel()
        {
            SignalManager.Get<OpenProjectSuccessSignal>().AddListener(OpenProjectSuccess);
            SignalManager.Get<CloseProjectSuccessSignal>().AddListener(CloseProjectSuccess);
            SignalManager.Get<ExitSuccessSignal>().AddListener(ExitSuccess);
            SignalManager.Get<NewFileSuccessSignal>().AddListener(NewFileSuccess);
            SignalManager.Get<SaveAllSuccessSignal>().AddListener(SaveAllSuccess);
            SignalManager.Get<SaveSuccessSignal>().AddListener(SaveSuccess);
            SignalManager.Get<LoadConfigSuccessSignal>().AddListener(LoadConfigSuccess);
        }

        ~MainWindowViewModel()
        {
            SignalManager.Get<OpenProjectSuccessSignal>().RemoveListener(OpenProjectSuccess);
            SignalManager.Get<CloseProjectSuccessSignal>().RemoveListener(CloseProjectSuccess);
            SignalManager.Get<ExitSuccessSignal>().RemoveListener(ExitSuccess);
            SignalManager.Get<NewFileSuccessSignal>().RemoveListener(NewFileSuccess);
            SignalManager.Get<SaveAllSuccessSignal>().RemoveListener(SaveAllSuccess);
            SignalManager.Get<SaveSuccessSignal>().RemoveListener(SaveSuccess);
            SignalManager.Get<LoadConfigSuccessSignal>().RemoveListener(LoadConfigSuccess);
        }

        private void LoadConfigSuccess()
        {
            LoadDefaultProject();
        }

        private void LoadDefaultProject()
        {
            var config = ModelManager.Get<NESToolConfigurationModel>();

            var openProjectCommand = new OpenProjectCommand();
            if (openProjectCommand.CanExecute(config.DefaultProjectPath))
            {
                openProjectCommand.Execute(config.DefaultProjectPath);
            }
        }

        private void CloseProjectSuccess()
        {
            //
        }

        private void OpenProjectSuccess(List<ProjectItem> list)
        {
            ProjectItems = list;
        }

        private void ExitSuccess()
        {
            Application.Current.Shutdown();
        }

        private void NewFileSuccess()
        {
            //
        }

        private void SaveAllSuccess()
        {
            //
        }

        private void SaveSuccess()
        {
            //
        }
    }
}
