using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class MapViewModel : ItemViewModel
    {
        private FileModelVO[] _banks;
        private int _selectedBank;
        private PaletteIndex _paletteIndex = 0;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private int _selectedAttributeTile = -1;
        private ImageSource _frameImage;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private bool _doNotSave = false;
        private Visibility _gridVisibility = Visibility.Visible;
        private string _metaData = string.Empty;
        private bool _exportAttributeTable = true;
        private WriteableBitmap _mapBitmap;
        private FileModelVO[] _palettes;
        private int _selectedPalette1 = -1;
        private int _selectedPalette2 = -1;
        private int _selectedPalette3 = -1;
        private int _selectedPalette4 = -1;
        private ObservableCollection<FileModelVO> _mapElements = new ObservableCollection<FileModelVO>();
        private ObservableCollection<KeyValuePair<string, string>> _properties = new ObservableCollection<KeyValuePair<string, string>>();
        private KeyValuePair<string, string> _selectedProperty;
        private int _selectedPropertyIndex;
        private string _editableSelectedProperty;
        private int _selectedMapElement = -1;

        public static Dictionary<Tuple<int, PaletteIndex>, Dictionary<Color, Color>> GroupedPalettes;
        public static TileUpdate[] FlagMapBitmapChanges;
        public static Point[] PointMapBitmapChanges;

        public PaletteIndex[] SpritePaletteIndices { get; } = new PaletteIndex[MapModel.MetaTileMax];

        public MapModel GetModel()
        {
            return ProjectItem?.FileHandler.FileModel is MapModel model ? model : null;
        }

        #region Commands
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public OpenAddMapElementCommand OpenAddMapElementCommand { get; } = new OpenAddMapElementCommand();
        public DeleteSelectedMapElement DeleteSelectedMapElement { get; } = new DeleteSelectedMapElement();
        public MoveUpSelectedMapElement MoveUpSelectedMapElement { get; } = new MoveUpSelectedMapElement();
        public MoveDownSelectedMapElement MoveDownSelectedMapElement { get; } = new MoveDownSelectedMapElement();
        public SaveSelectedPropertyValue SaveSelectedPropertyValue { get; } = new SaveSelectedPropertyValue();
        #endregion

        #region get/set
        public Visibility GridVisibility
        {
            get => _gridVisibility;
            set
            {
                _gridVisibility = value;

                OnPropertyChanged("GridVisibility");
            }
        }

        public string EditableSelectedProperty
        {
            get => _editableSelectedProperty;
            set
            {
                _editableSelectedProperty = value;

                OnPropertyChanged("EditableSelectedProperty");
            }
        }

        public int SelectedPropertyIndex
        {
            get => _selectedPropertyIndex;
            set
            {
                _selectedPropertyIndex = value;

                OnPropertyChanged("SelectedPropertyIndex");
            }
        }

        public KeyValuePair<string, string> SelectedProperty
        {
            get => _selectedProperty;
            set
            {
                _selectedProperty = value;

                OnPropertyChanged("SelectedProperty");

                if (_selectedProperty.Key != null && _selectedProperty.Value != null)
                {
                    EditableSelectedProperty = _selectedProperty.Value;
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> Properties
        {
            get => _properties;
            set
            {
                _properties = value;

                OnPropertyChanged("Properties");
            }
        }

        public bool ExportAttributeTable
        {
            get => _exportAttributeTable;
            set
            {
                if (_exportAttributeTable != value)
                {
                    _exportAttributeTable = value;

                    UpdateAndSaveExportAttributeTable(value);
                }

                OnPropertyChanged("ExportAttributeTable");
            }
        }

        public ObservableCollection<FileModelVO> MapElements
        {
            get => _mapElements;
            set
            {
                _mapElements = value;

                OnPropertyChanged("MapElements");
            }
        }

        public int SelectedMapElement
        {
            get => _selectedMapElement;
            set
            {
                _selectedMapElement = value;

                OnPropertyChanged("SelectedMapElement");

                EditableSelectedProperty = "";

                SelectedProperty = new KeyValuePair<string, string>();

                if (SelectedMapElement != -1)
                {
                    for (int i = 0; i < GetModel().Entities.Count; i++)
                    {
                        Entity entity = GetModel().Entities[i];

                        if (entity.SortIndex == SelectedMapElement)
                        {
                            Properties = new ObservableCollection<KeyValuePair<string, string>>(GetModel().Entities[i].Properties);
                        }
                    }
                }
                else
                {
                    if (Properties.Count > 0)
                    {
                        Properties.Clear();
                    }
                }
            }
        }

        public string MetaData
        {
            get => _metaData;
            set
            {
                if (_metaData != value)
                {
                    _metaData = value;

                    UpdateAndSaveMetaData(value);
                }

                OnPropertyChanged("MetaData");
            }
        }

        public ImageSource FrameImage
        {
            get => _frameImage;
            set
            {
                _frameImage = value;

                OnPropertyChanged("FrameImage");
            }
        }

        public int SelectedAttributeTile
        {
            get => _selectedAttributeTile;
            set
            {
                _selectedAttributeTile = value;

                OnPropertyChanged("SelectedAttributeTile");
            }
        }

        public int SelectedPalette1
        {
            get => _selectedPalette1;
            set
            {
                if (_selectedPalette1 != value)
                {
                    _selectedPalette1 = value;

                    UpdateAndSavePalette(value, PaletteIndex.Palette0);
                }

                OnPropertyChanged("SelectedPalette1");
            }
        }

        public int SelectedPalette2
        {
            get => _selectedPalette2;
            set
            {
                if (_selectedPalette2 != value)
                {
                    _selectedPalette2 = value;

                    UpdateAndSavePalette(value, PaletteIndex.Palette1);
                }

                OnPropertyChanged("SelectedPalette2");
            }
        }

        public int SelectedPalette3
        {
            get => _selectedPalette3;
            set
            {
                if (_selectedPalette3 != value)
                {
                    _selectedPalette3 = value;

                    UpdateAndSavePalette(value, PaletteIndex.Palette2);
                }

                OnPropertyChanged("SelectedPalette3");
            }
        }

        public int SelectedPalette4
        {
            get => _selectedPalette4;
            set
            {
                if (_selectedPalette4 != value)
                {
                    _selectedPalette4 = value;

                    UpdateAndSavePalette(value, PaletteIndex.Palette3);
                }

                OnPropertyChanged("SelectedPalette4");
            }
        }

        public FileModelVO[] Palettes
        {
            get => _palettes;
            set
            {
                _palettes = value;

                OnPropertyChanged("Palettes");
            }
        }

        public PaletteIndex PaletteIndex
        {
            get => _paletteIndex;
            set
            {
                if (_paletteIndex != value)
                {
                    _paletteIndex = value;

                    SavePaletteIndex(value);
                }

                OnPropertyChanged("PaletteIndex");
            }
        }

        public int SelectedBank
        {
            get => _selectedBank;
            set
            {
                _selectedBank = value;

                OnPropertyChanged("SelectedBank");
            }
        }

        public FileModelVO[] Banks
        {
            get => _banks;
            set
            {
                _banks = value;

                OnPropertyChanged("Banks");
            }
        }

        public double RectangleLeft
        {
            get => _rectangleLeft;
            set
            {
                _rectangleLeft = value;

                OnPropertyChanged("RectangleLeft");
            }
        }

        public double RectangleTop
        {
            get => _rectangleTop;
            set
            {
                _rectangleTop = value;

                OnPropertyChanged("RectangleTop");
            }
        }

        public Visibility RectangleVisibility
        {
            get { return _rectangleVisibility; }
            set
            {
                _rectangleVisibility = value;

                OnPropertyChanged("RectangleVisibility");
            }
        }
        #endregion

        private void UpdateAndSaveMetaData(string metaData)
        {
            GetModel().MetaData = metaData;

            if (_doNotSave)
            {
                return;
            }

            ProjectItem.FileHandler.Save();
        }

        private void UpdateAndSaveExportAttributeTable(bool exportAttributeTable)
        {
            GetModel().ExportAttributeTable = exportAttributeTable;

            ProjectItem.FileHandler.Save();
        }

        private void UpdateAndSavePalette(int newValue, PaletteIndex index)
        {
            GetModel().PaletteIDs[(int)index] = newValue == -1 ? string.Empty : Palettes[newValue + 1].Model.GUID;

            if (!_doNotSave)
            {
                PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(GetModel().PaletteIDs[(int)index]);
                if (paletteModel != null)
                {
                    SetPalleteWithColors(paletteModel, index);
                }
                else
                {
                    SetPaletteEmpty(index);
                }

                ProjectItem.FileHandler.Save();
            }
        }

        public MapViewModel()
        {
            UpdateDialogInfo();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            GridVisibility = MainWindow.ToolBarMapShowHideGrid ? Visibility.Visible : Visibility.Hidden;

            FlagMapBitmapChanges = new TileUpdate[MapModel.MetaTileMax];
            PointMapBitmapChanges = new Point[MapModel.MetaTileMax];

            for (int i = 0; i < FlagMapBitmapChanges.Length; ++i)
            {
                FlagMapBitmapChanges[i] = TileUpdate.None;
            }

            GroupedPalettes = new Dictionary<Tuple<int, PaletteIndex>, Dictionary<Color, Color>>();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener += OnColorPaletteControlSelected;
            SignalManager.Get<UpdateMapImageSignal>().Listener += OnUpdateMapImage;
            SignalManager.Get<SelectPaletteIndexSignal>().Listener += OnSelectPaletteIndex;
            SignalManager.Get<MapPaintToolSignal>().Listener += OnMapPaintTool;
            SignalManager.Get<MapEraseToolSignal>().Listener += OnMapEraseTool;
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener += OnFileModelVOSelectionChanged;
            SignalManager.Get<ShowGridSignal>().Listener += OnShowGrid;
            SignalManager.Get<HideGridSignal>().Listener += OnHideGrid;
            SignalManager.Get<DeleteSelectedMapElementSignal>().Listener += OnDeleteSelectedMapElement;
            SignalManager.Get<MoveUpSelectedMapElementSignal>().Listener += OnMoveUpSelectedMapElement;
            SignalManager.Get<MoveDownSelectedMapElementSignal>().Listener += OnMoveDownSelectedMapElement;
            SignalManager.Get<AddMapElementSignal>().Listener += OnAddMapElement;
            SignalManager.Get<SaveSelectedPropertyValueSignal>().Listener += OnSaveSelectedPropertyValue;
            #endregion

            _doNotSave = true;

            if (GetModel() != null)
            {
                ExportAttributeTable = GetModel().ExportAttributeTable;
                MetaData = GetModel().MetaData;
            }

            LoadPalettes();
            LoadEntities();

            LoadPaletteIndex(0);
            LoadPaletteIndex(1);
            LoadPaletteIndex(2);
            LoadPaletteIndex(3);

            _doNotSave = false;

            LoadFrameImage(false);
            LoadMetaTileProperties();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener -= OnColorPaletteControlSelected;
            SignalManager.Get<UpdateMapImageSignal>().Listener -= OnUpdateMapImage;
            SignalManager.Get<SelectPaletteIndexSignal>().Listener -= OnSelectPaletteIndex;
            SignalManager.Get<MapPaintToolSignal>().Listener -= OnMapPaintTool;
            SignalManager.Get<MapEraseToolSignal>().Listener -= OnMapEraseTool;
            SignalManager.Get<ShowGridSignal>().Listener -= OnShowGrid;
            SignalManager.Get<HideGridSignal>().Listener -= OnHideGrid;
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener -= OnFileModelVOSelectionChanged;
            SignalManager.Get<DeleteSelectedMapElementSignal>().Listener -= OnDeleteSelectedMapElement;
            SignalManager.Get<MoveUpSelectedMapElementSignal>().Listener -= OnMoveUpSelectedMapElement;
            SignalManager.Get<MoveDownSelectedMapElementSignal>().Listener -= OnMoveDownSelectedMapElement;
            SignalManager.Get<AddMapElementSignal>().Listener -= OnAddMapElement;
            SignalManager.Get<SaveSelectedPropertyValueSignal>().Listener -= OnSaveSelectedPropertyValue;
            #endregion
        }

        private void SetPaletteEmpty(PaletteIndex index)
        {
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 0);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 1);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 2);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 3);
        }

        private void SetPalleteWithColors(PaletteModel paletteModel, PaletteIndex index)
        {
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color0), index, 0);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color1), index, 1);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color2), index, 2);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color3), index, 3);
        }

        private void OnHideGrid()
        {
            if (!IsActive)
            {
                return;
            }

            GridVisibility = Visibility.Hidden;
        }

        private void OnShowGrid()
        {
            if (!IsActive)
            {
                return;
            }

            GridVisibility = Visibility.Visible;
        }

        private void LoadPaletteIndex(int index)
        {
            if (GetModel() == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(GetModel().PaletteIDs[index]))
            {
                for (int i = 0; i < Palettes.Length; ++i)
                {
                    FileModelVO item = Palettes[i];

                    if (item.Model == null)
                    {
                        continue;
                    }

                    if (item.Model.GUID == GetModel().PaletteIDs[index])
                    {
                        switch (index)
                        {
                            case 0: SelectedPalette1 = i - 1; break;
                            case 1: SelectedPalette2 = i - 1; break;
                            case 2: SelectedPalette3 = i - 1; break;
                            case 3: SelectedPalette4 = i - 1; break;
                        }
                    }
                }
            }
        }

        private void LoadMetaTileProperties()
        {
            if (GetModel() == null)
            {
                return;
            }

            for (int i = 0; i < GetModel().AttributeTable.Length; ++i)
            {
                SpritePaletteIndices[i] = (PaletteIndex)GetModel().AttributeTable[i].PaletteIndex;
            }
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            if (!IsActive)
            {
                return;
            }

            if (_doNotSave)
            {
                return;
            }

            GroupedPalettes.Clear();

            LoadFrameImage(false);
        }

        private void OnSelectPaletteIndex(PaletteIndex paletteIndex)
        {
            if (!IsActive)
            {
                return;
            }

            PaletteIndex = paletteIndex;
        }

        private void OnMapPaintTool()
        {
            if (!IsActive)
            {
                return;
            }

            RectangleVisibility = Visibility.Hidden;
        }
        private void OnMapEraseTool()
        {
            if (!IsActive)
            {
                return;
            }

            RectangleVisibility = Visibility.Hidden;
        }

        private void LoadPalettes()
        {
            if (GetModel() == null)
            {
                return;
            }

            for (int i = 0; i < 4; ++i)
            {
                string paletteId = GetModel().PaletteIDs[i];

                PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);
                if (paletteModel == null)
                {
                    SetPaletteEmpty((PaletteIndex)i);
                }
                else
                {
                    SetPalleteWithColors(paletteModel, (PaletteIndex)i);
                }
            }
        }

        private void LoadEntities()
        {
            if (GetModel() == null)
            {
                return;
            }

            foreach (Entity entity in GetModel().Entities)
            {
                FileModelVO fileModelVO = ProjectFiles.GetFileModel(entity.GUID);

                if (fileModelVO == null)
                {
                    return;
                }

                fileModelVO.Index = entity.SortIndex;

                MapElements.Add(fileModelVO);
            }
        }

        private void OnSaveSelectedPropertyValue(string newPropertyValue)
        {
            if (!IsActive)
            {
                return;
            }

            if (string.IsNullOrEmpty(newPropertyValue))
            {
                return;
            }

            Dictionary<string, string> properties = GetModel().Entities[SelectedMapElement - 1].Properties;

            int count = 0;
            foreach (KeyValuePair<string, string> property in properties)
            {
                if (count == SelectedPropertyIndex)
                {
                    properties[property.Key] = newPropertyValue;
                    break;
                }

                count++;
            }

            // setting the same value to trigger the population of the properties in the view again.
            SelectedMapElement = SelectedMapElement;

            ProjectItem.FileHandler.Save();
        }

        private void OnAddMapElement(string entityId)
        {
            if (!IsActive)
            {
                return;
            }

            if (string.IsNullOrEmpty(entityId))
            {
                return;
            }

            FileModelVO fileModelVO = ProjectFiles.GetFileModel(entityId);

            if (fileModelVO == null)
            {
                return;
            }

            fileModelVO.Index = MapElements.Count + 1;

            MapElements.Add(fileModelVO);

            Entity entity = new Entity()
            {
                GUID = entityId,
                SortIndex = fileModelVO.Index,
                X = 0,
                Y = 0,
                Properties = new Dictionary<string, string>()
            };

            EntityModel entityModel = fileModelVO.Model as EntityModel;

            foreach (string property in entityModel.Properties)
            {
                entity.Properties.Add(property, string.Empty);
            }

            GetModel().Entities.Add(entity);

            ProjectItem.FileHandler.Save();
        }

        private void OnMoveDownSelectedMapElement(int selectedElementIndex)
        {
            if (!IsActive)
            {
                return;
            }

            if (SelectedMapElement < MapElements.Count)
            {
                AdjustOrderOfMapElements(selectedElementIndex, selectedElementIndex - 1);

                SelectedMapElement = selectedElementIndex + 1;
            }
        }

        private void OnMoveUpSelectedMapElement(int selectedElementIndex)
        {
            if (!IsActive)
            {
                return;
            }

            if (SelectedMapElement > 1)
            {
                AdjustOrderOfMapElements(selectedElementIndex - 2, selectedElementIndex - 1);

                SelectedMapElement = selectedElementIndex - 1;
            }
        }

        private void AdjustOrderOfMapElements(int newSelectedIndex, int selectedElementIndex)
        {
            ObservableCollection<FileModelVO> cacheList = new ObservableCollection<FileModelVO>(MapElements);

            FileModelVO cacheElement = cacheList[newSelectedIndex];
            cacheList[newSelectedIndex] = cacheList[selectedElementIndex];
            cacheList[newSelectedIndex].Index = newSelectedIndex + 1;

            cacheList[selectedElementIndex] = cacheElement;
            cacheList[selectedElementIndex].Index = selectedElementIndex + 1;

            MapElements = cacheList;

            Entity cacheEntity = GetModel().Entities[newSelectedIndex];

            Entity tmp = GetModel().Entities[selectedElementIndex];
            tmp.SortIndex = newSelectedIndex + 1;
            GetModel().Entities[newSelectedIndex] = tmp;

            cacheEntity.SortIndex = selectedElementIndex + 1;
            GetModel().Entities[selectedElementIndex] = cacheEntity;

            ProjectItem.FileHandler.Save();
        }

        private void OnDeleteSelectedMapElement(int selectedElementIndex)
        {
            if (!IsActive)
            {
                return;
            }

            // remove the element in the view
            foreach (FileModelVO vo in MapElements)
            {
                if (vo.Index == selectedElementIndex)
                {
                    if (MapElements.Remove(vo))
                    {
                        SelectedMapElement = -1;

                        break;
                    }
                }
            }

            // adjust the indeces in the view
            foreach (FileModelVO vo in MapElements)
            {
                if (vo.Index > selectedElementIndex)
                {
                    vo.Index--;
                }
            }

            // now update the model
            foreach (Entity entity in GetModel().Entities)
            {
                if (entity.SortIndex == selectedElementIndex)
                {
                    if (GetModel().Entities.Remove(entity))
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < GetModel().Entities.Count; i++)
            {
                Entity entity = GetModel().Entities[i];

                if (entity.SortIndex > selectedElementIndex)
                {
                    entity.SortIndex--;

                    GetModel().Entities[i] = entity;
                }
            }

            ProjectItem.FileHandler.Save();
        }

        private void OnColorPaletteControlSelected(Color color, PaletteIndex paletteIndex, int colorPosition)
        {
            if (!IsActive)
            {
                return;
            }

            if (_doNotSave)
            {
                return;
            }

            int colorInt = ((color.R & 0xff) << 16) | ((color.G & 0xff) << 8) | (color.B & 0xff);

            int prevColorInt = 0;

            string paletteId = GetModel().PaletteIDs[(int)paletteIndex];

            PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);
            if (paletteModel != null)
            {
                switch (colorPosition)
                {
                    case 0:
                        prevColorInt = paletteModel.Color0;
                        paletteModel.Color0 = colorInt;
                        break;
                    case 1:
                        prevColorInt = paletteModel.Color1;
                        paletteModel.Color1 = colorInt;
                        break;
                    case 2:
                        prevColorInt = paletteModel.Color2;
                        paletteModel.Color2 = colorInt;
                        break;
                    case 3:
                        prevColorInt = paletteModel.Color3;
                        paletteModel.Color3 = colorInt;
                        break;
                }

                ProjectFiles.SaveModel(paletteId, paletteModel);
            }

            ProjectItem.FileHandler.Save();

            Color prevColor = Util.GetColorFromInt(prevColorInt);

            AdjustPaletteCache(paletteIndex, colorPosition, prevColor, color);

            SignalManager.Get<UpdateMapImageSignal>().Dispatch();
        }

        private void AdjustPaletteCache(PaletteIndex paletteIndex, int colorPosition, Color prevColor, Color color)
        {
            foreach (KeyValuePair<Tuple<int, PaletteIndex>, Dictionary<Color, Color>> entry in GroupedPalettes)
            {
                Tuple<int, PaletteIndex> tuple = entry.Key;

                if (tuple.Item2 == paletteIndex)
                {
                    int index = 0;
                    foreach (KeyValuePair<Color, Color> entry2 in entry.Value)
                    {
                        if (index == colorPosition && entry2.Value == prevColor)
                        {
                            entry.Value[entry2.Key] = color;
                            break;
                        }

                        index++;
                    }
                }
            }
        }

        private void UpdateDialogInfo()
        {
            List<FileModelVO> list = new List<FileModelVO>
            {
                new FileModelVO()
                {
                    Index = -1,
                    Name = "None",
                    Model = null
                }
            };

            list.AddRange(ProjectFiles.GetModels<PaletteModel>());

            Palettes = list.ToArray();

            int index = 0;

            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<BankModel>().ToArray()
                .Where(p => (p.Model as BankModel).BankUseType == BankUseType.Background);

            Banks = new FileModelVO[banks.Count()];

            foreach (FileModelVO item in banks)
            {
                item.Index = index;

                Banks[index] = item;

                index++;
            }
        }

        private void OnUpdateMapImage()
        {
            if (!IsActive)
            {
                return;
            }

            LoadFrameImage(false);
        }

        public void LoadFrameImage(bool update)
        {
            if (GetModel() == null)
            {
                return;
            }

            MapUtils.CreateImage(GetModel(), ref _mapBitmap, update);

            FrameImage = _mapBitmap;
        }

        public void SavePaletteIndex(PaletteIndex index)
        {
            if (SelectedAttributeTile == -1)
            {
                return;
            }

            if (SpritePaletteIndices[SelectedAttributeTile] != index)
            {
                SpritePaletteIndices[SelectedAttributeTile] = index;

                GetModel().AttributeTable[SelectedAttributeTile].PaletteIndex = (int)index;

                ProjectItem.FileHandler.Save();

                FlagMapBitmapChanges[SelectedAttributeTile] = TileUpdate.Normal;

                LoadFrameImage(true);
            }
        }
    }
}
