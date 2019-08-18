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
        private readonly int[] _spritePaletteIndices = new int[240];

        public static Dictionary<string, WriteableBitmap> FrameBitmapCache;
        public static Dictionary<Tuple<int, int>, Dictionary<Color, Color>> GroupedPalettes;

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

                OnPropertyChanged("SelectedFrameTile");
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

            FrameBitmapCache = new Dictionary<string, WriteableBitmap>();
            GroupedPalettes = new Dictionary<Tuple<int, int>, Dictionary<Color, Color>>();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
            SignalManager.Get<UpdateMapImageSignal>().AddListener(OnUpdateMapImage);
            SignalManager.Get<SelectPaletteIndexSignal>().AddListener(OnSelectPaletteIndex);
            #endregion

            LoadPalettes();
            LoadFrameImage();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().RemoveListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().RemoveListener(OnColorPaletteControlSelected);
            SignalManager.Get<UpdateMapImageSignal>().RemoveListener(OnUpdateMapImage);
            SignalManager.Get<SelectPaletteIndexSignal>().RemoveListener(OnSelectPaletteIndex);
            #endregion
        }

        private void OnSelectPaletteIndex(PaletteIndex paletteIndex)
        {
            PaletteIndex = paletteIndex;
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

            AdjustPaletteCache(paletteIndex, prevColor, color);

            SignalManager.Get<UpdateMapImageSignal>().Dispatch();
        }

        private void AdjustPaletteCache(int paletteIndex, Color prevColor, Color color)
        {
            foreach (KeyValuePair<Tuple<int, int>, Dictionary<Color, Color>> entry in GroupedPalettes)
            {
                Tuple<int, int> tuple = entry.Key as Tuple<int, int>;

                if (tuple.Item2 == paletteIndex)
                {
                    foreach (KeyValuePair<Color, Color> entry2 in entry.Value)
                    {
                        if (entry2.Value == prevColor)
                        {
                            entry.Value[entry2.Key] = color;
                            break;
                        }
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
                RectangleLeft = point.X;
                RectangleTop = point.Y;

                if (SelectionRectangleVisibility == Visibility.Visible)
                {
                    int index = ((int)point.X / 8) + (((int)point.Y / 8) * 8);

                    SelectedAttributeTile = index / 4;

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

                    LoadFrameImage();
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

        private void OnUpdateMapImage()
        {
            LoadFrameImage();
        }

        private void LoadFrameImage()
        {
            if (GetModel() == null)
            {
                return;
            }

            WriteableBitmap frameBitmap = MapUtils.CreateImage(GetModel());

            if (frameBitmap != null)
            {
                FrameImage = Util.ConvertWriteableBitmapToBitmapImage(frameBitmap);
            }
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

                LoadFrameImage();
            }
        }
    }
}
