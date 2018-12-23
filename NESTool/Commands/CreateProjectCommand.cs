using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using System.IO;
using System.Windows;

namespace NESTool.Commands
{
    public class CreateProjectCommand : Command
    {
        private const string _projectFileNameKey = "projectFileName";
        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";
        private const string _folderPatternTablesKey = "folderPatternTables";

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

            var prgSize = (int)values[2];
            var chrSize = (int)values[3];
            var mapperId = (int)values[4];

            var projectFullPath = Path.Combine(path, projectName);

            CreateProject(projectFullPath, prgSize, chrSize, mapperId);

            SignalManager.Get<CreateProjectSuccessSignal>().Dispatch(projectFullPath);
        }

        private void CreateProject(string projectFullPath, int prgSize, int chrSize, int mapperIndex)
        {
            Directory.CreateDirectory(projectFullPath);

            var projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

            var folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            var folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            var folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            var folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
            var folderPatternTables = (string)Application.Current.FindResource(_folderPatternTablesKey);

            string fullPathToProjectFile = Path.Combine(projectFullPath, projectFileName);

            File.Create(fullPathToProjectFile).Dispose();

            FileSystemManager.CreateFolder(folderBanks, projectFullPath);
            FileSystemManager.CreateFolder(folderCharacters, projectFullPath);
            FileSystemManager.CreateFolder(folderMaps, projectFullPath);
            FileSystemManager.CreateFolder(folderTileSets, projectFullPath);
            FileSystemManager.CreateFolder(folderPatternTables, projectFullPath);

            var model = ModelManager.Get<ProjectModel>();

            // In case there is already a model loaded, we want to reset it to its default state
            model.Reset();

            model.Header.CHRSize = chrSize;
            model.Header.PRGSize = prgSize;
            model.Header.INesMapper = mapperIndex;

            model.Save(fullPathToProjectFile);
        }
    }
}
