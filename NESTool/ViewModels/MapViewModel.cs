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

        public static Dictionary<string, WriteableBitmap> FrameBitmapCache;
        public static Dictionary<Tuple<int, int>, Dictionary<Color, Color>> GroupedPalettes;
        public static bool[] FlagMapBitmapChanges;

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

        public MapViewModel()
        {
            UpdateDialogInfo();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            FlagMapBitmapChanges = new bool[MapModel.MetaTileMax];

            for (int i = 0; i < FlagMapBitmapChanges.Length; ++i)
            {
                FlagMapBitmapChanges[i] = false;
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
            #endregion

            LoadPalettes();
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
            #endregion
        }

        private void LoadMetaTileProperties()
        {
            for (int i = 0; i < GetModel().AttributeTable.Length; ++i)
            {
                _spritePaletteIndices[i] = GetModel().AttributeTable[i].PaletteIndex;
            }
        }

        private void OnSelectPaletteIndex(PaletteIndex paletteIndex)
        {
            PaletteIndex = paletteIndex;
        }

        private void OnMapPaintTool()
        {
            RectangleVisibility = Visibility.Hidden;
        }
        private void OnMapEraseTool()
        {
            RectangleVisibility = Visibility.Hidden;
        }

        private void LoadPalettes()
        {
            _doNotSavePalettes = true;

            byte R;
            byte G;
            byte B;

            // Load palettes
            for (int i = 0; i < 4; ++i)
            {
                int color0 = GetModel().Palettes[i].Color0;
                R = (byte)(color0 >> 16);
                G = (byte)(color0 >> 8);
                B = (byte)color0;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 0);

                int color1 = GetModel().Palettes[i].Color1;
                R = (byte)(color1 >> 16);
                G = (byte)(color1 >> 8);
                B = (byte)color1;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 1);

                int color2 = GetModel().Palettes[i].Color2;
                R = (byte)(color2 >> 16);
                G = (byte)(color2 >> 8);
                B = (byte)color2;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 2);

                int color3 = GetModel().Palettes[i].Color3;
                R = (byte)(color3 >> 16);
                G = (byte)(color3 >> 8);
                B = (byte)color3;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 3);
            }

            _doNotSavePalettes = false;
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

            switch (colorPosition)
            {
                case 0:
                    prevColorInt = GetModel().Palettes[paletteIndex].Color0;
                    GetModel().Palettes[paletteIndex].Color0 = colorInt;
                    break;
                case 1:
                    prevColorInt = GetModel().Palettes[paletteIndex].Color1;
                    GetModel().Palettes[paletteIndex].Color1 = colorInt;
                    break;
                case 2:
                    prevColorInt = GetModel().Palettes[paletteIndex].Color2;
                    GetModel().Palettes[paletteIndex].Color2 = colorInt;
                    break;
                case 3:
                    prevColorInt = GetModel().Palettes[paletteIndex].Color3;
                    GetModel().Palettes[paletteIndex].Color3 = colorInt;
                    break;
            }

            ProjectItem.FileHandler.Save();

            // Convert previous color to type Color
            byte R = (byte)(prevColorInt >> 16);
            byte G = (byte)(prevColorInt >> 8);
            byte B = (byte)prevColorInt;

            Color prevColor = Color.FromRgb(R, G, B);

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
            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<PatternTableModel>().ToArray()
                .Where(p => (p.Model as PatternTableModel).PatternTableType == PatternTableType.Background);

            Banks = new FileModelVO[banks.Count()];

            int index = 0;

            foreach (FileModelVO item in banks)
            {
                item.Id = index;

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

            if (!(Banks[SelectedBank].Model is PatternTableModel model))
            {
                return;
            }

            WriteableBitmap patternTableBitmap = PatternTableUtils.CreateImage(model, ref _bitmapCache);

            BankImage = Util.ConvertWriteableBitmapToBitmapImage(patternTableBitmap);
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (!IsActive)
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
            FlagMapBitmapChanges[SelectedAttributeTile] = true;

            // Paint
            Point characterPoint = new Point
            {
                X = RectangleLeft,
                Y = RectangleTop
            };

            PatternTableModel model = Banks[SelectedBank].Model as PatternTableModel;

            string guid = model.PTTiles[SelectedPatternTableTile].GUID;

            ref MapTile tile = ref GetModel().GetTile(index);

            tile.Point = characterPoint;
            tile.BankID = model.GUID;
            tile.BankTileID = guid;

            ProjectItem?.FileHandler.Save();

            LoadFrameImage(true);
        }

        private void EraseTile(int index)
        {
            FlagMapBitmapChanges[SelectedAttributeTile] = true;

            ref MapTile tile = ref GetModel().GetTile(index);

            tile.BankID = string.Empty;
            tile.BankTileID = string.Empty;

            ProjectItem?.FileHandler.Save();

            LoadFrameImage(false);
        }

        private void OnUpdateMapImage()
        {
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

                FlagMapBitmapChanges[SelectedAttributeTile] = true;

                LoadFrameImage(true);
            }
        }
    }
}
