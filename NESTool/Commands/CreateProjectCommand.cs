using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
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
        private const string _folderPalettesKey = "folderPalettes";
        private const string _folderWorldsKey = "folderWorlds";

        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;
            string path = (string)values[0];
            string projectName = (string)values[1];

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
            object[] values = (object[])parameter;
            string path = (string)values[0];
            string projectName = (string)values[1];

            int prgSize = (int)values[2];
            int chrSize = (int)values[3];
            int mapperId = (int)values[4];

            string projectFullPath = Path.Combine(path, projectName);

            CreateProject(projectFullPath, prgSize, chrSize, mapperId);

            SignalManager.Get<CloseProjectSuccessSignal>().Dispatch();
            SignalManager.Get<CreateProjectSuccessSignal>().Dispatch(projectFullPath);
        }

        private void CreateProject(string projectFullPath, int prgSize, int chrSize, int mapperIndex)
        {
            Directory.CreateDirectory(projectFullPath);

            string projectFileName = (string)Application.Current.FindResource(_projectFileNameKey);

            string folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            string folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            string folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            string folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
            string folderPalettes = (string)Application.Current.FindResource(_folderPalettesKey);
            string folderWorlds = (string)Application.Current.FindResource(_folderWorldsKey);

            string fullPathToProjectFile = Path.Combine(projectFullPath, projectFileName);

            File.Create(fullPathToProjectFile).Dispose();

            Directory.CreateDirectory(Path.Combine(projectFullPath, folderBanks));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderCharacters));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderMaps));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderTileSets));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderPalettes));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderWorlds));

            ProjectModel model = ModelManager.Get<ProjectModel>();

            // In case there is already a model loaded, we want to reset it to its default state
            model.Reset();

            model.Header.CHRSize = chrSize;
            model.Header.PRGSize = prgSize;
            model.Header.INesMapper = mapperIndex;

            model.Save(fullPathToProjectFile);
        }
    }
}
