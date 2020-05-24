﻿using ArchitectureLibrary.Signals;
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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class MapViewModel : ItemViewModel
    {
        private FileModelVO[] _banks;
        private int _selectedBank;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private ImageSource _bankImage;
        private Visibility _selectionRectangleVisibility = Visibility.Hidden;
        private double _selectionRectangleTop = 0.0;
        private double _selectionRectangleLeft = 0.0;
        private int _selectedPatternTableTile;
        private PaletteIndex _paletteIndex = 0;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private int _selectedAttributeTile = -1;
        private ImageSource _frameImage;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private bool _doNotSavePalettes = false;
        private readonly int[] _spritePaletteIndices = new int[MapModel.MetaTileMax];
        private WriteableBitmap _mapBitmap;
        private FileModelVO[] _palettes;
        private int _selectedPalette1 = -1;
        private int _selectedPalette2 = -1;
        private int _selectedPalette3 = -1;
        private int _selectedPalette4 = -1;

        public static Dictionary<string, WriteableBitmap> FrameBitmapCache;
        public static Dictionary<Tuple<int, int>, Dictionary<Color, Color>> GroupedPalettes;
        public static TileUpdate[] FlagMapBitmapChanges;
        public static Point[] PointMapBitmapChanges;

        public MapModel GetModel()
        {
            if (ProjectItem?.FileHandler.FileModel is MapModel model)
            {
                return model;
            }

            return null;
        }

        #region Commands
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        #endregion

        #region get/set
        public int SelectedPatternTableTile
        {
            get { return _selectedPatternTableTile; }
            set
            {
                _selectedPatternTableTile = value;

                OnPropertyChanged("SelectedPatternTableTile");
            }
        }

        public ImageSource FrameImage
        {
            get
            {
                return _frameImage;
            }
            set
            {
                _frameImage = value;

                OnPropertyChanged("FrameImage");
            }
        }

        public int SelectedAttributeTile
        {
            get { return _selectedAttributeTile; }
            set
            {
                _selectedAttributeTile = value;

                OnPropertyChanged("SelectedAttributeTile");
            }
        }

        public int SelectedPalette1
        {
            get { return _selectedPalette1; }
            set
            {
                if (_selectedPalette1 != value)
                {
                    _selectedPalette1 = value;

                    UpdateAndSavePalette(value, 0);
                }

                OnPropertyChanged("SelectedPalette1");
            }
        }

        public int SelectedPalette2
        {
            get { return _selectedPalette2; }
            set
            {
                if (_selectedPalette2 != value)
                {
                    _selectedPalette2 = value;

                    UpdateAndSavePalette(value, 1);
                }

                OnPropertyChanged("SelectedPalette2");
            }
        }

        public int SelectedPalette3
        {
            get { return _selectedPalette3; }
            set
            {
                if (_selectedPalette3 != value)
                {
                    _selectedPalette3 = value;

                    UpdateAndSavePalette(value, 2);
                }

                OnPropertyChanged("SelectedPalette3");
            }
        }

        public int SelectedPalette4
        {
            get { return _selectedPalette4; }
            set
            {
                if (_selectedPalette4 != value)
                {
                    _selectedPalette4 = value;

                    UpdateAndSavePalette(value, 3);
                }

                OnPropertyChanged("SelectedPalette4");
            }
        }

        public FileModelVO[] Palettes
        {
            get { return _palettes; }
            set
            {
                _palettes = value;

                OnPropertyChanged("Palettes");
            }
        }

        public PaletteIndex PaletteIndex
        {
            get { return _paletteIndex; }
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

        public double SelectionRectangleLeft
        {
            get { return _selectionRectangleLeft; }
            set
            {
                _selectionRectangleLeft = value;

                OnPropertyChanged("SelectionRectangleLeft");
            }
        }

        public double SelectionRectangleTop
        {
            get { return _selectionRectangleTop; }
            set
            {
                _selectionRectangleTop = value;

                OnPropertyChanged("SelectionRectangleTop");
            }
        }

        public Visibility SelectionRectangleVisibility
        {
            get { return _selectionRectangleVisibility; }
            set
            {
                _selectionRectangleVisibility = value;

                OnPropertyChanged("SelectionRectangleVisibility");
            }
        }

        public int SelectedBank
        {
            get { return _selectedBank; }
            set
            {
                _selectedBank = value;

                OnPropertyChanged("SelectedBank");
            }
        }

        public ImageSource BankImage
        {
            get
            {
                return _bankImage;
            }
            set
            {
                _bankImage = value;

                OnPropertyChanged("BankImage");
            }
        }

        public FileModelVO[] Banks
        {
            get { return _banks; }
            set
            {
                _banks = value;

                OnPropertyChanged("Banks");
            }
        }

        public double RectangleLeft
        {
            get { return _rectangleLeft; }
            set
            {
                _rectangleLeft = value;

                OnPropertyChanged("RectangleLeft");
            }
        }

        public double RectangleTop
        {
            get { return _rectangleTop; }
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

        private void UpdateAndSavePalette(int newValue, int index)
        {
            if (newValue == -1)
            {
                GetModel().PaletteIDs[index] = string.Empty;
            }
            else
            {
                GetModel().PaletteIDs[index] = Palettes[newValue + 1].Model.GUID;
            }

            if (!_doNotSavePalettes)
            {
                PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(GetModel().PaletteIDs[index]);
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

            FlagMapBitmapChanges = new TileUpdate[MapModel.MetaTileMax];
            PointMapBitmapChanges = new Point[MapModel.MetaTileMax];

            for (int i = 0; i < FlagMapBitmapChanges.Length; ++i)
            {
                FlagMapBitmapChanges[i] = TileUpdate.None;
            }

            FrameBitmapCache = new Dictionary<string, WriteableBitmap>();
            GroupedPalettes = new Dictionary<Tuple<int, int>, Dictionary<Color, Color>>();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
            SignalManager.Get<UpdateMapImageSignal>().AddListener(OnUpdateMapImage);
            SignalManager.Get<SelectPaletteIndexSignal>().AddListener(OnSelectPaletteIndex);
            SignalManager.Get<MapPaintToolSignal>().AddListener(OnMapPaintTool);
            SignalManager.Get<MapEraseToolSignal>().AddListener(OnMapEraseTool);
            SignalManager.Get<ProjectItemLoadedSignal>().AddListener(OnProjectItemLoaded);
            SignalManager.Get<FileModelVOSelectionChangedSignal>().AddListener(OnFileModelVOSelectionChanged);
            #endregion

            _doNotSavePalettes = true;

            LoadPalettes();

            LoadPaletteIndex(0);
            LoadPaletteIndex(1);
            LoadPaletteIndex(2);
            LoadPaletteIndex(3);

            _doNotSavePalettes = false;

            LoadFrameImage(false);
            LoadMetaTileProperties();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().RemoveListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().RemoveListener(OnColorPaletteControlSelected);
            SignalManager.Get<UpdateMapImageSignal>().RemoveListener(OnUpdateMapImage);
            SignalManager.Get<SelectPaletteIndexSignal>().RemoveListener(OnSelectPaletteIndex);
            SignalManager.Get<MapPaintToolSignal>().RemoveListener(OnMapPaintTool);
            SignalManager.Get<MapEraseToolSignal>().RemoveListener(OnMapEraseTool);
            SignalManager.Get<ProjectItemLoadedSignal>().RemoveListener(OnProjectItemLoaded);
            SignalManager.Get<FileModelVOSelectionChangedSignal>().RemoveListener(OnFileModelVOSelectionChanged);
            #endregion
        }

        private void SetPaletteEmpty(int index)
        {
			SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 0);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 1);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 2);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(0, 0, 0), index, 3);
        }

        private void SetPalleteWithColors(PaletteModel paletteModel, int index)
        {
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color0), index, 0);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color1), index, 1);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color2), index, 2);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Util.GetColorFromInt(paletteModel.Color3), index, 3);
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
                _spritePaletteIndices[i] = GetModel().AttributeTable[i].PaletteIndex;
            }
        }

        private void OnProjectItemLoaded(string path, string name)
        {
            if (!IsActive)
            {
                return;
            }

            if (ProjectItem.FileHandler.Path != path || ProjectItem.FileHandler.Name != name)
            {
                return;
            }
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            if (!IsActive)
            {
                return;
            }

            if (_doNotSavePalettes)
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
                    SetPaletteEmpty(i);
                }
                else
                {
                    SetPalleteWithColors(paletteModel, i);
                }
            }
        }

        private void OnColorPaletteControlSelected(Color color, int paletteIndex, int colorPosition)
        {
            if (!IsActive)
            {
                return;
            }

            if (_doNotSavePalettes)
            {
                return;
            }

            int colorInt = ((color.R & 0xff) << 16) | ((color.G & 0xff) << 8) | (color.B & 0xff);

            int prevColorInt = 0;

            string paletteId = GetModel().PaletteIDs[paletteIndex];

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

        private void AdjustPaletteCache(int paletteIndex, int colorPosition, Color prevColor, Color color)
        {
            foreach (KeyValuePair<Tuple<int, int>, Dictionary<Color, Color>> entry in GroupedPalettes)
            {
                Tuple<int, int> tuple = entry.Key as Tuple<int, int>;

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
            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<BankModel>().ToArray()
                .Where(p => (p.Model as BankModel).PatternTableType == PatternTableType.Background);

            Banks = new FileModelVO[banks.Count()];

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

            foreach (FileModelVO item in banks)
            {
                item.Index = index;

                Banks[index] = item;

                index++;
            }

            LoadBankImage();
        }

        private void LoadBankImage()
        {
            if (Banks.Length == 0)
            {
                return;
            }

            if (!(Banks[SelectedBank].Model is BankModel model))
            {
                return;
            }

            WriteableBitmap patternTableBitmap = BanksUtils.CreateImage(model, ref _bitmapCache);

            BankImage = Util.ConvertWriteableBitmapToBitmapImage(patternTableBitmap);
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (!IsActive)
            {
                return;
            }

            if (point.X < 0 || point.Y < 0)
            {
                return;
            }

            if (sender.Name == "imgFrame")
            {
                int index = ((int)point.X / 8) + (((int)point.Y / 8) * 32);

                RectangleLeft = point.X;
                RectangleTop = point.Y;

                (int, int) tuple = GetModel().GetAttributeTileIndex(index);

                SelectedAttributeTile = tuple.Item1;

                if (MainWindow.ToolBarMapTool == EditFrameTools.Paint && SelectionRectangleVisibility == Visibility.Visible)
                {
                    PaintTile(index);
                }
                else if (MainWindow.ToolBarMapTool == EditFrameTools.Erase)
                {
                    EraseTile(index);
                }
                else if (MainWindow.ToolBarMapTool == EditFrameTools.Select)
                {
                    // get the first element in the selected meta tile
                    int[] array = GetModel().GetMetaTableArray(tuple.Item1);

                    if (array != null)
                    {
                        // from the first element, get the corresponding X, Y coordinates to put the rectangle in the right place
                        int y = (array[0] / 32) * 8;
                        int x = (array[0] - (32 * (array[0] / 32))) * 8;

                        RectangleLeft = x;
                        RectangleTop = y;

                        SelectTile();
                    }
                }
            }
            else if (sender.Name == "imgPatternTable")
            {
                SelectionRectangleVisibility = Visibility.Visible;
                SelectionRectangleLeft = point.X;
                SelectionRectangleTop = point.Y;

                int index = ((int)point.X / 8) + (((int)point.Y / 8) * 16);

                SelectedPatternTableTile = index;
            }
        }

        private void SelectTile()
        {
            if (SelectedAttributeTile == -1)
            {
                return;
            }

            RectangleVisibility = Visibility.Visible;

            SignalManager.Get<SelectPaletteIndexSignal>().Dispatch((PaletteIndex)_spritePaletteIndices[SelectedAttributeTile]);
        }

        private void PaintTile(int index)
        {
            FlagMapBitmapChanges[SelectedAttributeTile] = TileUpdate.Normal;

            Point selectedTilePoint = new Point
            {
                X = RectangleLeft,
                Y = RectangleTop
            };

            BankModel model = Banks[SelectedBank].Model as BankModel;

            string guid = model.PTTiles[SelectedPatternTableTile].GUID;

            ref MapTile tile = ref GetModel().GetTile(index);

            tile.Point = selectedTilePoint;
            tile.BankID = model.GUID;
            tile.BankTileID = guid;

            PointMapBitmapChanges[SelectedAttributeTile] = selectedTilePoint;

            ProjectItem?.FileHandler.Save();

            LoadFrameImage(true);
        }

        private void EraseTile(int index)
        {
            FlagMapBitmapChanges[SelectedAttributeTile] = TileUpdate.Erased;

            Point selectedTilePoint = new Point
            {
                X = RectangleLeft,
                Y = RectangleTop
            };

            PointMapBitmapChanges[SelectedAttributeTile] = selectedTilePoint;

            ref MapTile tile = ref GetModel().GetTile(index);

            LoadFrameImage(true);

            tile.BankID = string.Empty;
            tile.BankTileID = string.Empty;

            ProjectItem?.FileHandler.Save();
        }

        private void OnUpdateMapImage()
        {
            if (!IsActive)
            {
                return;
            }

            LoadFrameImage(false);
        }

        private void LoadFrameImage(bool update)
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

            if (_spritePaletteIndices[SelectedAttributeTile] != (int)index)
            {
                _spritePaletteIndices[SelectedAttributeTile] = (int)index;

                GetModel().AttributeTable[SelectedAttributeTile].PaletteIndex = (int)index;

                ProjectItem.FileHandler.Save();

                FlagMapBitmapChanges[SelectedAttributeTile] = TileUpdate.Normal;

                LoadFrameImage(true);
            }
        }
    }
}
