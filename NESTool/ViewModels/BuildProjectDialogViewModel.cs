using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System.Collections.Generic;
using System.Linq;

namespace NESTool.ViewModels
{
    public class BuildProjectDialogViewModel : ViewModel
    {
        private string _folderPath;
        private FileModelVO[] _patternTableSprites;
        private FileModelVO[] _patternTableBackgrounds;
        private int _selectedPatternTableSprite;
        private int _selectedPatternTableBackground;
		private ElementPaletteModel _selectedPalette;
		private ElementPaletteModel _selectedSpritePalette;
		private ElementPaletteModel _selectedBackgroundPalette;

		public List<ElementPaletteModel> ElementPalettes { get; set; } = new List<ElementPaletteModel>();
		public List<ElementPaletteModel> ElementBackgroundPalettes { get; set; } = new List<ElementPaletteModel>();
		public List<ElementPaletteModel> ElementSpritePalettes { get; set; } = new List<ElementPaletteModel>();

		#region Commands
		public BuildProjectCommand BuildProjectCommand { get; } = new BuildProjectCommand();
        public BrowseFolderCommand BrowseFolderCommand { get; } = new BrowseFolderCommand();
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
		#endregion

		#region get/set
		public ElementPaletteModel SelectedPalette
		{
			get { return _selectedPalette; }
			set
			{
				_selectedPalette = value;
				OnPropertyChanged("SelectedPalette");
			}
		}

		public ElementPaletteModel SelectedBackgroundPalette
		{
			get { return _selectedBackgroundPalette; }
			set
			{
				_selectedBackgroundPalette = value;
				OnPropertyChanged("SelectedBackgroundPalette");
			}
		}

		public ElementPaletteModel SelectedSpritePalette
		{
			get { return _selectedSpritePalette; }
			set
			{
				_selectedSpritePalette = value;
				OnPropertyChanged("SelectedSpritePalette");
			}
		}

		public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        public FileModelVO[] PatternTableSprites
        {
            get { return _patternTableSprites; }
            set
            {
                _patternTableSprites = value;

                OnPropertyChanged("PatternTableSprites");
            }
        }

        public FileModelVO[] PatternTableBackgrounds
        {
            get { return _patternTableBackgrounds; }
            set
            {
                _patternTableBackgrounds = value;

                OnPropertyChanged("PatternTableBackgrounds");
            }
        }

        public int SelectedPatternTableSprite
        {
            get { return _selectedPatternTableSprite; }
            set
            {
                _selectedPatternTableSprite = value;

                ProjectModel project = ModelManager.Get<ProjectModel>();

                if (value > 0)
                {
                    if (project.Build.PatternTableSpriteId != PatternTableSprites[value].Model.GUID)
                    {
                        project.Build.PatternTableSpriteId = PatternTableSprites[value].Model.GUID;
                        project.Save();
                    }
                }
                else
                {
                    project.Build.PatternTableSpriteId = "";
                    project.Save();
                }

                OnPropertyChanged("SelectedPatternTableSprite");
            }
        }

        public int SelectedPatternTableBackground
        {
            get { return _selectedPatternTableBackground; }
            set
            {
                _selectedPatternTableBackground = value;

                ProjectModel project = ModelManager.Get<ProjectModel>();

                if (value > 0)
                {
                    if (project.Build.PatternTableBackgroundId != PatternTableBackgrounds[value].Model.GUID)
                    {
                        project.Build.PatternTableBackgroundId = PatternTableBackgrounds[value].Model.GUID;
                        project.Save();
                    }
                }
                else
                {
                    project.Build.PatternTableBackgroundId = "";
                    project.Save();
                }

                OnPropertyChanged("SelectedPatternTableBackground");
            }
        }
		#endregion

        public BuildProjectDialogViewModel()
        {
			#region Signals
			SignalManager.Get<BrowseFolderSuccessSignal>().AddListener(BrowseFolderSuccess);
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);
			#endregion

			ProjectModel project = ModelManager.Get<ProjectModel>();

            FolderPath = project.Build.OutputFilePath;

            CreatePatternTableArrays();

            int index = 0;
            foreach (FileModelVO vo in PatternTableSprites)
            {
                if (vo.Model?.GUID == project.Build.PatternTableSpriteId)
                {
                    SelectedPatternTableSprite = index;
                    break;
                }

                index++;
            }

            index = 0;
            foreach (FileModelVO vo in PatternTableBackgrounds)
            {
                if (vo.Model?.GUID == project.Build.PatternTableBackgroundId)
                {
                    SelectedPatternTableBackground = index;
                    break;
                }

                index++;
            }

			InitializePalettes();
		}

		private void InitializePalettes()
		{
			List<FileModelVO> models = ProjectFiles.GetModels<PaletteModel>();

			foreach (FileModelVO item in models)
			{
				PaletteModel model = item.Model as PaletteModel;

				ElementPalettes.Add(new ElementPaletteModel()
				{
					Name = item.Name,
					Model = item.Model as PaletteModel
				});
			}

			ProjectModel project = ModelManager.Get<ProjectModel>();

			// TODO: read from ProjectModel
			ElementBackgroundPalettes.Add(new ElementPaletteModel() { Name = "gato 1" });
			ElementBackgroundPalettes.Add(new ElementPaletteModel() { Name = "gato 2" });
			ElementBackgroundPalettes.Add(new ElementPaletteModel() { Name = "gato 3" });
			ElementBackgroundPalettes.Add(new ElementPaletteModel() { Name = "gato 4" });

			ElementSpritePalettes.Add(new ElementPaletteModel() { Name = "gato 1" });
			ElementSpritePalettes.Add(new ElementPaletteModel() { Name = "gato 2" });
			ElementSpritePalettes.Add(new ElementPaletteModel() { Name = "gato 3" });
			ElementSpritePalettes.Add(new ElementPaletteModel() { Name = "gato 4" });
		}

		private void CreatePatternTableArrays()
        {
            List<FileModelVO> patternTableSprites = new List<FileModelVO>
            {
                new FileModelVO() { Id = 0, Name = "None", Model = null }
            };

            List<FileModelVO> patternTableBackgrounds = new List<FileModelVO>
            {
                new FileModelVO() { Id = 0, Name = "None", Model = null }
            };

            IEnumerable<FileModelVO> sprites = ProjectFiles.GetModels<BankModel>().ToArray().Where(p => (p.Model as BankModel).PatternTableType == PatternTableType.Characters);
            IEnumerable<FileModelVO> backgrounds = ProjectFiles.GetModels<BankModel>().ToArray().Where(p => (p.Model as BankModel).PatternTableType == PatternTableType.Background);

            int index = 1;

            foreach (FileModelVO item in sprites)
            {
                item.Id = index++;

                patternTableSprites.Add(item);
            }

            index = 1;

            foreach (FileModelVO item in backgrounds)
            {
                item.Id = index++;

                patternTableBackgrounds.Add(item);
            }

            PatternTableSprites = patternTableSprites.ToArray();
            PatternTableBackgrounds = patternTableBackgrounds.ToArray();
        }

        private void OnCloseDialog()
        {
			#region Signals
			SignalManager.Get<BrowseFolderSuccessSignal>().RemoveListener(BrowseFolderSuccess);
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
			#endregion
		}

		private void BrowseFolderSuccess(string folderPath)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            if (project.Build.OutputFilePath != folderPath)
            {
                FolderPath = folderPath;

                project.Build.OutputFilePath = folderPath;

                project.Save();
            }
        }
    }
}
