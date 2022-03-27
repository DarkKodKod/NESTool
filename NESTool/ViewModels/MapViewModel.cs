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
            #endregion

            _doNotSave = true;

            if (GetModel() != null)
            {
                ExportAttributeTable = GetModel().ExportAttributeTable;
                MetaData = GetModel().MetaData;
            }

            LoadPalettes();

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

            FrameImage = Util.ConvertWriteableBitmapToBitmapImage(_mapBitmap);
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
