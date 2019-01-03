using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using System.Collections.Generic;
using NESTool.Enums;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class ElementDialogViewModel : ViewModel
    {
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
        public CreateElementCommand CreateElementCommand { get; } = new CreateElementCommand();

        public List<ElementTypeModel> ElementTypes { get; set; } = new List<ElementTypeModel>();

        public ElementTypeModel SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";
        private const string _folderPatternTablesKey = "folderPatternTables";

        private ElementTypeModel _selectedType;

        public ElementDialogViewModel()
        {
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);

            InitializeElements();
        }

        private void OnCloseDialog()
        {
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        private void InitializeElements()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            string banks = (string)Application.Current.FindResource(_folderBanksKey);
            string characters = (string)Application.Current.FindResource(_folderCharactersKey);
            string maps = (string)Application.Current.FindResource(_folderMapsKey);
            string tileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
            string patternTables = (string)Application.Current.FindResource(_folderPatternTablesKey);

            ElementTypes.Add(new ElementTypeModel()
            {
                Name = banks,
                Image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Bank.png", UriKind.RelativeOrAbsolute)),
                Type = ProjectItemType.Bank,
                Path = Path.Combine(projectModel.ProjectPath, banks)
            });
            ElementTypes.Add(new ElementTypeModel()
            {
                Name = characters,
                Image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/character.png", UriKind.RelativeOrAbsolute)),
                Type = ProjectItemType.Character,
                Path = Path.Combine(projectModel.ProjectPath, characters)
            });
            ElementTypes.Add(new ElementTypeModel()
            {
                Name = maps,
                Image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/map.png", UriKind.RelativeOrAbsolute)),
                Type = ProjectItemType.Map,
                Path = Path.Combine(projectModel.ProjectPath, maps)
            });
            ElementTypes.Add(new ElementTypeModel()
            {
                Name = patternTables,
                Image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/pattern_table.png", UriKind.RelativeOrAbsolute)),
                Type = ProjectItemType.PatternTable,
                Path = Path.Combine(projectModel.ProjectPath, patternTables)
            });
            ElementTypes.Add(new ElementTypeModel()
            {
                Name = tileSets,
                Image = new BitmapImage(new Uri(@"pack://application:,,,/Resources/tileset.png", UriKind.RelativeOrAbsolute)),
                Type = ProjectItemType.TileSet,
                Path = Path.Combine(projectModel.ProjectPath, tileSets)
            });
        }
    }
}
