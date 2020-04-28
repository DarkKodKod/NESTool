using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
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
        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";
        private const string _folderPalettesKey = "folderPalettes";

        private readonly string _folderBanks;
        private readonly string _folderCharacters;
        private readonly string _folderMaps;
        private readonly string _folderTileSets;
        private readonly string _folderPalettes;

        public OpenProjectCommand()
        {
            _folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            _folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            _folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            _folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
            _folderPalettes = (string)Application.Current.FindResource(_folderPalettesKey);
        }

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
                string projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

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
                string projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

                string fullPath = Path.Combine(path, projectFileName);

                if (File.Exists(fullPath))
                {
                    // Extract the name of the folder as our project name
                    int startIndex = path.LastIndexOf("\\");
                    string projectName = path.Substring(startIndex + 1, path.Length - startIndex - 1);

                    LoadProject(path, fullPath, projectName);
                }
            }
            else
            {
                // We want to capture the browse folder signal to open the project
                SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);

                using (BrowseFolderCommand browseFolder = new BrowseFolderCommand())
                {
                    browseFolder.Execute(null);
                }   

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
            // Clean up previous stuff
            ProjectFiles.Handlers.Clear();

            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            if (projectModel.IsOpen())
            {
                SignalManager.Get<CloseProjectSuccessSignal>().Dispatch();
            }

            // load project configuration file
            projectModel.Load(directoryPath, projectFullPath);

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
                // Discard any unknown folder in the root of the project
                if (directory.Name != _folderBanks &&
                    directory.Name != _folderCharacters &&
                    directory.Name != _folderMaps &&
                    directory.Name != _folderTileSets &&
                    directory.Name != _folderPalettes &&
                    parent == null)
                {
                    continue;
                }

                ProjectItem item = new ProjectItem()
                {
                    DisplayName = directory.Name,
                    IsLoaded = true
                };

                string ext = "";

                if (extension == string.Empty)
                {
                    ext = Util.GetFolderExtension(directory.Name);

                    item.Parent = null;
                    item.IsRoot = true;
                }
                else
                {
                    ext = extension;

                    item.Parent = parent;
                    item.IsRoot = false;
                }

                item.Type = Util.GetItemType(ext);
                item.IsFolder = true;

                DirectoryInfo parentFolder = Directory.GetParent(directory.FullName);

                SignalManager.Get<RegisterFileHandlerSignal>().Dispatch(item, parentFolder.FullName);

                // Check if it was some folders inside
                DirectoryInfo[] subFolders = directory.GetDirectories();
                if (subFolders.Length > 0)
                {
                    List<ProjectItem> subItems = new List<ProjectItem>();

                    ScanDirectories(subFolders, ref subItems, item, ext);

                    foreach (ProjectItem element in subItems)
                    {
                        item.Items.Add(element);
                    }
                }

                // Check files
                FileInfo[] Files = directory.GetFiles($"*{ext}");

                foreach (FileInfo file in Files)
                {
                    string displayName = Path.GetFileNameWithoutExtension(file.Name);

                    ProjectItem fileItem = new ProjectItem()
                    {
                        DisplayName = displayName,
                        Type = Util.GetItemType(ext),
                        IsLoaded = true
                    };

                    fileItem.Parent = item;

                    item.Items.Add(fileItem);

                    SignalManager.Get<RegisterFileHandlerSignal>().Dispatch(fileItem, file.DirectoryName);
                }

                projectItems.Add(item);
            }
        }

        private void UpdateConfigurations(string projectFullPath)
        {
            NESToolConfigurationModel model = ModelManager.Get<NESToolConfigurationModel>();

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
