using NESTool.Architecture.Commands;
using NESTool.Architecture.Model;
using NESTool.Architecture.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.ViewModels;
using NESTool.VOs;
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
                var projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

                path = Path.Combine(path, projectFileName);

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
                var projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

                string fullPath = Path.Combine(path, projectFileName);

                if (File.Exists(fullPath))
                {
                    // Extract the name of the folder as our project name
                    int startIndex = path.LastIndexOf("\\");
                    var projectName = path.Substring(startIndex + 1, path.Length - startIndex - 1);

                    LoadProject(path, fullPath, projectName);
                }
            }
        }

        private void LoadProject(string directoryPath, string projectFullPath, string projectName)
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

            SignalManager.Get<OpenProjectSuccessSignal>().Dispatch(new ProjectOpenVO() { Items = projectItems, ProjectName = projectName });
        }
    }
}
