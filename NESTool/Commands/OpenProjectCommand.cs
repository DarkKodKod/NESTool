using NESTool.Architecture.Commands;
using NESTool.Architecture.Model;
using NESTool.Architecture.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace NESTool.Commands
{
    public class OpenProjectCommand : Command
    {
        private const string _projectFileNameKey = "projectFileName";

        public override bool CanExecute(object parameter)
        {
            string path = parameter as string;

            bool pathIsNull = string.IsNullOrWhiteSpace(path);

            // when path is empty we can open open project dialog
            if (pathIsNull)
            {
                return true;
            }
            else
            {
                // Check if the project file exists in the folder before open the project
                var projectName = (string)Application.Current.FindResource(_projectFileNameKey);

                path = Path.Combine(path, projectName);

                if (File.Exists(path))
                {
                    return true;
                }

                return false;
            }
        }

        public override void Execute(object parameter)
        {
            string path = parameter as string;

            // if there is something to load
            if (!string.IsNullOrWhiteSpace(path))
            {
                // Check if the project file exists in the folder before open the project
                var projectName = (string)Application.Current.FindResource(_projectFileNameKey);

                string fullPath = Path.Combine(path, projectName);

                if (File.Exists(fullPath))
                {
                    LoadProject(path, fullPath);
                }
            }
        }

        private void LoadProject(string directoryPath, string projectFullPath)
        {
            var projectModel = ModelManager.Get<ProjectModel>();

            // load project configuration file
            projectModel.Load(projectFullPath);

            // load project folder
            DirectoryInfo d = new DirectoryInfo(directoryPath);

            DirectoryInfo[] directories = d.GetDirectories();

            List<ProjectItem> projectItems = new List<ProjectItem>();

            foreach (DirectoryInfo directory in directories)
            {
                projectItems.Add(new ProjectItem(directory.Name));
            }

            SignalManager.Get<OpenProjectSuccessSignal>().Dispatch(projectItems);
        }
    }
}
