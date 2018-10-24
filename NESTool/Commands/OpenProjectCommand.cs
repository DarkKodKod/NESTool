using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels.ProjectItems;
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

            // It is ok for the path to be null, that means we want to open the folder dialog to find the project path
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
            else
            {
                // We want to capture the brow folder signal to open the project
                SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);

                BrowseFolderCommand browseFolder = new BrowseFolderCommand();
                browseFolder.Execute(null);

                SignalManager.Get<BrowseFolderSuccessSignal>().RemoveListener(BrowseFolderSuccess);
            }
        }

        private void BrowseFolderSuccess(string path)
        {
            if (CanExecute(path))
            {
                Execute(path);
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

            ScanDirectories(directories, ref projectItems);

            SignalManager.Get<OpenProjectSuccessSignal>().Dispatch(new ProjectOpenVO() { Items = projectItems, ProjectName = projectName });

            UpdateConfigurations(directoryPath);
        }

        private void ScanDirectories(DirectoryInfo[] directories, ref List<ProjectItem> projectItems, ProjectItem parent = null, string extension = "")
        {
            foreach (DirectoryInfo directory in directories)
            {
                var item = new ProjectFolder(directory.Name, directory.FullName, ProjectItemType.Folder);

                string ext = "";

                if (extension == string.Empty)
                {
                    ext = Util.GetFolderExtension(directory.Name);

                    item.Parent = null;
                    item.Root = true;
                }
                else
                {
                    ext = extension;

                    item.Parent = parent;
                    item.Root = false;
                }

                item.Group = Util.GetItemType(ext);

                // Check if it was some folders inside
                DirectoryInfo[] subFolders = directory.GetDirectories();
                if (subFolders.Length > 0)
                {
                    List<ProjectItem> subItems = new List<ProjectItem>();

                    ScanDirectories(subFolders, ref subItems, item, ext);

                    foreach (var element in subItems)
                    {
                        item.Items.Add(element);
                    }
                }

                // Check files
                FileInfo[] Files = directory.GetFiles($"*{ext}");

                foreach (FileInfo file in Files)
                {
                    var displayName = Path.GetFileNameWithoutExtension(file.Name);

                    var fileItem = new ProjectItem(displayName, file.FullName, Util.GetItemType(ext));

                    fileItem.Parent = item;

                    item.Items.Add(fileItem);
                }

                projectItems.Add(item);
            }
        }

        private void UpdateConfigurations(string projectFullPath)
        {
            var model = ModelManager.Get<NESToolConfigurationModel>();

            if (model.DefaultProjectPath != projectFullPath)
            {
                // Update the recent projects also with the new project path
                model.InsertToRecentProjects(projectFullPath);

                // Make this new project the default project
                model.DefaultProjectPath = projectFullPath;

                model.Save();
            }

            SignalManager.Get<UpdateRecentProjectsSignal>().Dispatch(model.RecentProjects);
        }
    }
}
