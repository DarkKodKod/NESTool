﻿using NESTool.Architecture.Commands;
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
        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";

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

            var folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            var folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            var folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            var folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);

            string fullPathToProjectFile = Path.Combine(projectFullPath, projectFileName);

            File.Create(fullPathToProjectFile).Dispose();

            Directory.CreateDirectory(Path.Combine(projectFullPath, folderBanks));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderCharacters));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderMaps));
            Directory.CreateDirectory(Path.Combine(projectFullPath, folderTileSets));

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
