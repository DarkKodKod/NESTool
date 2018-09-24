using NESTool.Architecture.Commands;
using NESTool.Architecture.Model;
using NESTool.Architecture.Signals;
using NESTool.Models;
using NESTool.Signals;
using System.IO;
using System.Windows;

namespace NESTool.Commands
{
    public class CreateProjectCommand : Command
    {
        private const string _projectFileNameKey = "projectFileName";

        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            var values = (object[])parameter;
            var path = (string)values[0];
            var projectName = (string)values[1];

            // It is needed the name of the project to continue
            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }

            // The path given needs to be valid
            if (!Directory.Exists(path))
            {
                return false;
            }

            // The full path to the new project needs to be brand new, if
            // the folder already exists, dont continue
            if (Directory.Exists(Path.Combine(path, projectName)))
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var values = (object[])parameter;
            var path = (string)values[0];
            var projectName = (string)values[1];

            var projectFullPath = Path.Combine(path, projectName);

            CreateProject(projectFullPath);

            UpdateConfigurations(projectFullPath);

            SignalManager.Get<CreateProjectSuccessSignal>().Dispatch(projectFullPath);
        }

        private void CreateProject(string projectFullPath)
        {
            Directory.CreateDirectory(projectFullPath);

            var projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

            string fullPathToProjectFile = Path.Combine(projectFullPath, projectFileName);

            File.Create(fullPathToProjectFile).Dispose();

            Directory.CreateDirectory(Path.Combine(projectFullPath, "Banks"));
            Directory.CreateDirectory(Path.Combine(projectFullPath, "Characters"));
            Directory.CreateDirectory(Path.Combine(projectFullPath, "Maps"));
            Directory.CreateDirectory(Path.Combine(projectFullPath, "TileSets"));

            var model = ModelManager.Get<ProjectModel>();

            // In case there is already a model loaded, we want to reset it to its default state
            model.Reset();

            model.Save(fullPathToProjectFile);
        }

        private void UpdateConfigurations(string projectFullPath)
        {
            var model = ModelManager.Get<NESToolConfigurationModel>();

            // Make this new project the default project
            model.DefaultProjectPath = projectFullPath;

            model.Save();
        }
    }
}
