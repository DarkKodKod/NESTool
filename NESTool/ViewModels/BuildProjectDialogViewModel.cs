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
using System.Collections.ObjectModel;
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
		public ObservableCollection<ElementPaletteModel> ElementBackgroundPalettes { get; set; } = new ObservableCollection<ElementPaletteModel>(new List<ElementPaletteModel>(4));
		public ObservableCollection<ElementPaletteModel> ElementSpritePalettes { get; set; } = new ObservableCollection<ElementPaletteModel>(new List<ElementPaletteModel>(4));

		#region Commands
		public BuildProjectCommand BuildProjectCommand { get; } = new BuildProjectCommand();
        public BrowseFolderCommand BrowseFolderCommand { get; } = new BrowseFolderCommand();
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
        public AddPaletteToListCommand AddPaletteToListCommand { get; } = new AddPaletteToListCommand();
        public DeletePaletteFromListCommand DeletePaletteFromListCommand { get; } = new DeletePaletteFromListCommand();
        public MovePaletteUpCommand MovePaletteUpCommand { get; } = new MovePaletteUpCommand();
        public MovePaletteDownCommand MovePaletteDownCommand { get; } = new MovePaletteDownCommand();
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
            SignalManager.Get<AddPaletteToListSignal>().AddListener(OnAddPaletteToList);
            SignalManager.Get<MovePaletteDownSignal>().AddListener(OnMovePaletteDown);
            SignalManager.Get<MovePaletteUpSignal>().AddListener(OnMovePaletteUp);
            SignalManager.Get<DeletePaletteFromListSignal>().AddListener(OnDeletePaletteFromList);
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

            List<string> output = new List<string>();
            bool changed = false;

			if (FillPaletteList(project.Build.BackgroundPalettes, ElementBackgroundPalettes, ref output))
			{
				project.Build.BackgroundPalettes = output.ToArray();
                changed = true;
            }

			if (FillPaletteList(project.Build.SpritePalettes, ElementSpritePalettes, ref output))
			{
				project.Build.SpritePalettes = output.ToArray();
                changed = true;
            }

            if (changed)
            {
                project.Save();
            }
        }

		private bool FillPaletteList(string[] originList, ObservableCollection<ElementPaletteModel> destination, ref List<string> output)
		{
            if (originList == null)
            {
                return false;
            }

            bool ret = false;

            output.Clear();

            foreach (string id in originList)
			{
				if (string.IsNullOrEmpty(id))
				{
					continue;
				}

				FileModelVO handler = ProjectFiles.GetFileModel(id);

				if (handler == null)
				{
                    // Model does not exist anymore, this means we will 
                    // copy the output array to the original arary
                    ret = true;

					continue;
				}

                output.Add(id);

                destination.Add(new ElementPaletteModel()
				{
					Name = handler.Name,
					Model = handler.Model as PaletteModel
                });
			}

			return ret;
		}

		private void CreatePatternTableArrays()
        {
            List<FileModelVO> patternTableSprites = new List<FileModelVO>
            {
                new FileModelVO() { Index = 0, Name = "None", Model = null }
            };

            List<FileModelVO> patternTableBackgrounds = new List<FileModelVO>
            {
                new FileModelVO() { Index = 0, Name = "None", Model = null }
            };

            IEnumerable<FileModelVO> sprites = ProjectFiles.GetModels<BankModel>().ToArray().Where(p => (p.Model as BankModel).PatternTableType == PatternTableType.Characters);
            IEnumerable<FileModelVO> backgrounds = ProjectFiles.GetModels<BankModel>().ToArray().Where(p => (p.Model as BankModel).PatternTableType == PatternTableType.Background);

            int index = 1;

            foreach (FileModelVO item in sprites)
            {
                item.Index = index++;

                patternTableSprites.Add(item);
            }

            index = 1;

            foreach (FileModelVO item in backgrounds)
            {
                item.Index = index++;

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
            SignalManager.Get<AddPaletteToListSignal>().RemoveListener(OnAddPaletteToList);
            SignalManager.Get<MovePaletteDownSignal>().RemoveListener(OnMovePaletteDown);
            SignalManager.Get<MovePaletteUpSignal>().RemoveListener(OnMovePaletteUp);
            SignalManager.Get<DeletePaletteFromListSignal>().RemoveListener(OnDeletePaletteFromList);
            #endregion
        }

        private ObservableCollection<ElementPaletteModel> MoveDownPalette(ElementPaletteModel palette, ObservableCollection<ElementPaletteModel> elementPalettes)
        {
            ElementPaletteModel tempItem = null;

            for (int i = 0; i < elementPalettes.Count; ++i)
            {
                ElementPaletteModel item = elementPalettes[i];

                if (item == palette)
                {
                    // last one
                    if (i == 3)
                    {
                        return null;
                    }

                    tempItem = item;
                }
                else if (tempItem != null && item != null)
                {
                    elementPalettes[i - 1] = elementPalettes[i];
                    elementPalettes[i] = tempItem;

                    tempItem = null;
                }
            }

            return elementPalettes;
        }

        private ObservableCollection<ElementPaletteModel> MoveUpPalette(ElementPaletteModel palette, ObservableCollection<ElementPaletteModel> elementPalettes)
        {
            ElementPaletteModel tempItem = null;

            for (int i = 3; i >= 0; --i)
            {
                if (i >= elementPalettes.Count)
                {
                    continue;
                }

                ElementPaletteModel item = elementPalettes[i];

                if (item == palette)
                {
                    // first one
                    if (i == 0)
                    {
                        return null;
                    }

                    tempItem = item;
                }
                else if (tempItem != null && item != null)
                {
                    elementPalettes[i + 1] = elementPalettes[i];
                    elementPalettes[i] = tempItem;

                    tempItem = null;
                }
            }

            return elementPalettes;
        }

        private void OnMovePaletteDown(ElementPaletteModel palette, PatternTableType type)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            if (type == PatternTableType.Background)
            {
                ObservableCollection<ElementPaletteModel> list = MoveDownPalette(palette, ElementBackgroundPalettes);

                if (list != null)
                {
                    ElementBackgroundPalettes = list;

                    List<string> idList = new List<string>();
                    foreach (ElementPaletteModel item in list)
                    {
                        idList.Add(item.Model.GUID);
                    }

                    project.Build.BackgroundPalettes = idList.ToArray();
                    project.Save();
                }
            }
            else if (type == PatternTableType.Characters)
            {
                ObservableCollection<ElementPaletteModel> list = MoveDownPalette(palette, ElementSpritePalettes);

                if (list != null)
                {
                    ElementSpritePalettes = list;

                    List<string> idList = new List<string>();
                    foreach (ElementPaletteModel item in list)
                    {
                        idList.Add(item.Model.GUID);
                    }

                    project.Build.SpritePalettes = idList.ToArray();
                    project.Save();
                }
            }
        }

        private void OnMovePaletteUp(ElementPaletteModel palette, PatternTableType type)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();
            
            if (type == PatternTableType.Background)
            {
                ObservableCollection<ElementPaletteModel> list = MoveUpPalette(palette, ElementBackgroundPalettes);

                if (list != null)
                {
                    ElementBackgroundPalettes = list;

                    List<string> idList = new List<string>();
                    foreach (ElementPaletteModel item in list)
                    {
                        idList.Add(item.Model.GUID);
                    }

                    project.Build.BackgroundPalettes = idList.ToArray();
                    project.Save();
                }
            }
            else if (type == PatternTableType.Characters)
            {
                ObservableCollection<ElementPaletteModel> list = MoveUpPalette(palette, ElementSpritePalettes);

                if (list != null)
                {
                    ElementSpritePalettes = list;

                    List<string> idList = new List<string>();
                    foreach (ElementPaletteModel item in list)
                    {
                        idList.Add(item.Model.GUID);
                    }

                    project.Build.SpritePalettes = idList.ToArray();
                    project.Save();
                }
            }
        }

        private void OnDeletePaletteFromList(ElementPaletteModel palette, PatternTableType type)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

			if (type == PatternTableType.Background)
            {
				if (ElementBackgroundPalettes.Contains(palette))
				{
					ElementBackgroundPalettes.Remove(palette);
                }

                List<string> list = project.Build.BackgroundPalettes.ToList();

                foreach (string item in list)
                {
                    if (item == palette.Model.GUID)
                    {
                        list.Remove(item);
                        list.Add(string.Empty);

                        project.Build.BackgroundPalettes = list.ToArray();

                        project.Save();

                        break;
                    }
                }

			}
            else if (type == PatternTableType.Characters)
            {
                if (ElementSpritePalettes.Contains(palette))
                {
                    ElementSpritePalettes.Remove(palette);
                }

                List<string> list = project.Build.SpritePalettes.ToList();

                foreach (string item in list)
                {
                    if (item == palette.Model.GUID)
                    {
                        list.Remove(item);
                        list.Add(string.Empty);

                        project.Build.SpritePalettes = list.ToArray();

                        project.Save();

                        break;
                    }
				}
			}
        }

        private void AddPaletteToList(int index, ElementPaletteModel palette, PatternTableType type)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            if (type == PatternTableType.Background)
            {
                project.Build.BackgroundPalettes[index] = palette.Model.GUID;

                ElementBackgroundPalettes.Add(new ElementPaletteModel()
                {
                    Name = palette.Name,
                    Model = palette.Model
                });
            }
            else if (type == PatternTableType.Characters)
            {
                project.Build.SpritePalettes[index] = palette.Model.GUID;

                ElementSpritePalettes.Add(new ElementPaletteModel()
                {
                    Name = palette.Name,
                    Model = palette.Model
                });
            }

            project.Save();
        }

        private void OnAddPaletteToList(ElementPaletteModel palette, PatternTableType type)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            int index = 0;

            if (type == PatternTableType.Background)
            {
                foreach (string item in project.Build.BackgroundPalettes)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        AddPaletteToList(index, palette, type);

                        return;
                    }

                    index++;
                }

                AddPaletteToList(index, palette, type);
            }
            else if (type == PatternTableType.Characters)
            {
                foreach (string item in project.Build.SpritePalettes)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        AddPaletteToList(index, palette, type);

                        return;
                    }

                    index++;
                }

                AddPaletteToList(index, palette, type);
            }
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
