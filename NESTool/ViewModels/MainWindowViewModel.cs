using NESTool.Architecture.Model;
using NESTool.Architecture.Signals;
using NESTool.Architecture.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
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
        public ShowAboutDialogCommand ShowAboutDialogCommand { get; } = new ShowAboutDialogCommand();
        public BuildProjectCommand BuildProjectCommand { get; } = new BuildProjectCommand();
        public OpenProjectPropertiesCommand OpenProjectPropertiesCommand { get; } = new OpenProjectPropertiesCommand();
        public ViewHelpCommand ViewHelpCommand { get; } = new ViewHelpCommand();

        private const string _projectNameKey = "applicationTitle";

        private string _title;
        private string _projectName;
        private List<ProjectItem> _projectItems;
        private List<RecentProjectModel> _recentProjects = new List<RecentProjectModel>();

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                OnPropertyChanged("ProjectName");
            }
        }

        public List<RecentProjectModel> RecentProjects
        {
            get { return _recentProjects; }
            set
            {
                _recentProjects = value;
                OnPropertyChanged("RecentProjects");
            }
        }

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
            SignalManager.Get<UpdateRecentProjectsSignal>().AddListener(UpdateRecentProjects);
            SignalManager.Get<BuildProjectSuccessSignal>().AddListener(BuildProjectSuccess);
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
            SignalManager.Get<UpdateRecentProjectsSignal>().RemoveListener(UpdateRecentProjects);
            SignalManager.Get<BuildProjectSuccessSignal>().RemoveListener(BuildProjectSuccess);
        }

        private void LoadConfigSuccess()
        {
            var model = ModelManager.Get<NESToolConfigurationModel>();

            UpdateRecentProjects(model.RecentProjects);

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

        private void UpdateRecentProjects(string[] recentProjects)
        {
            var list = new List<RecentProjectModel>();

            int index = 1;

            foreach (var project in recentProjects)
            {
                if (!string.IsNullOrEmpty(project))
                {
                    // Extract the name of the folder as our project name
                    int startIndex = project.LastIndexOf("\\");
                    var projectName = project.Substring(startIndex + 1, project.Length - startIndex - 1);

                    list.Add(new RecentProjectModel()
                    {
                        Path = project,
                        DisplayName = $"_{ index } {projectName} ({project})"
                    });

                    index++;
                }
            }

            RecentProjects = list;
        }

        private void OpenProjectSuccess(ProjectOpenVO vo)
        {
            ProjectItems = vo.Items;

            var projectName = (string)Application.Current.FindResource(_projectNameKey);

            ProjectName = vo.ProjectName;

            Title = $"{ vo.ProjectName } - { projectName }";
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

        private void CloseProjectSuccess()
        {
            ProjectName = "";
        }

        private void BuildProjectSuccess()
        {
            //
        }
    }
}
