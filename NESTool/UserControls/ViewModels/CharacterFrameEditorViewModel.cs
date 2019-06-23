using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.UserControls.ViewModels
{
    public class CharacterFrameEditorViewModel : ViewModel
    {
        private enum SpriteProperties
        {
            FlipX,
            FlipY,
            PaletteIndex
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ValueUnion
        {
            [FieldOffset(0)] public int integer;
            [FieldOffset(4)] public bool boolean;
        }

        private FileModelVO[] _banks;
        private int _selectedBank;
        private string _tabId;
        private int _frameIndex;
        private string _projectGridSize;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private ImageSource _bankImage;
        private Visibility _selectionRectangleVisibility = Visibility.Hidden;
        private double _selectionRectangleTop = 0.0;
        private double _selectionRectangleLeft = 0.0;
        private int _selectedPatternTableTile;
        private int _selectedFrameTile;
        private CharacterModel _characterModel;
        private FileHandler _fileHandler;
        private ImageSource _frameImage;
        private EditFrameTools _editFrameTools;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private bool _flipX = false;
        private bool _flipY = false;
        private PaletteIndex _paletteIndex = 0;
        private readonly bool[] _spritePropertiesX = new bool[64];
        private readonly bool[] _spritePropertiesY = new bool[64];
        private readonly int[] _spritePaletteIndices = new int[64];
        private bool _doNotSavePalettes = false;

        #region Commands
        public SwitchCharacterFrameViewCommand SwitchCharacterFrameViewCommand { get; } = new SwitchCharacterFrameViewCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        #endregion

        #region get/set
        public bool FlipX
        {
            get { return _flipX; }
            set
            {
                if (EditFrameTools != EditFrameTools.Select)
                {
                    return;
                }

                if (_flipX != value)
                {
                    _flipX = value;

                    OnPropertyChanged("FlipX");

                    SaveProperty(SpriteProperties.FlipX, new ValueUnion { boolean = value });
                }
            }
        }

        public bool FlipY
        {
            get { return _flipY; }
            set
            {
                if (EditFrameTools != EditFrameTools.Select)
                {
                    return;
                }

                if (_flipY != value)
                {
                    _flipY = value;

                    OnPropertyChanged("FlipY");

                    SaveProperty(SpriteProperties.FlipY, new ValueUnion { boolean = value });
                }
            }
        }

        public FileHandler FileHandler
        {
            get { return _fileHandler; }
            set
            {
                _fileHandler = value;

                OnPropertyChanged("FileHandler");
            }
        }

        public CharacterModel CharacterModel
        {
            get { return _characterModel; }
            set
            {
                _characterModel = value;

                OnPropertyChanged("CharacterModel");
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

                    SaveProperty(SpriteProperties.PaletteIndex, new ValueUnion { integer = (int)value });
                }

                OnPropertyChanged("PaletteIndex");
            }
        }

        public EditFrameTools EditFrameTools
        {
            get
            {
                return _editFrameTools;
            }
            set
            {
                if (_editFrameTools != value)
                {
                    _editFrameTools = value;

                    if (_editFrameTools != EditFrameTools.Select)
                    {
                        RectangleVisibility = Visibility.Hidden;
                    }

                    OnPropertyChanged("EditFrameTools");
                }
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

        public int SelectedFrameTile
        {
            get { return _selectedFrameTile; }
            set
            {
                _selectedFrameTile = value;

                OnPropertyChanged("SelectedFrameTile");
            }
        }

        public int SelectedPatternTableTile
        {
            get { return _selectedPatternTableTile; }
            set
            {
                _selectedPatternTableTile = value;

                OnPropertyChanged("SelectedPatternTableTile");
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

        public int SelectedBank
        {
            get { return _selectedBank; }
            set
            {
                _selectedBank = value;

                OnPropertyChanged("SelectedBank");
            }
        }

        public string ProjectGridSize
        {
            get
            {
                return _projectGridSize;
            }
            set
            {
                _projectGridSize = value;

                OnPropertyChanged("ProjectGridSize");
            }
        }

        public string TabID
        {
            get { return _tabId; }
            set
            {
                _tabId = value;

                OnPropertyChanged("TabID");

                for (int i = 0; i < CharacterModel.Animations.Length; ++i)
                {
                    if (CharacterModel.Animations[i].ID == TabID)
                    {
                        AnimationIndex = i;
                        break;
                    }
                }
            }
        }

        public int AnimationIndex { get; set; }

        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                _frameIndex = value;

                OnPropertyChanged("FrameIndex");
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

        public CharacterFrameEditorViewModel()
        {
            UpdateDialogInfo();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().AddListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
            #endregion

            EditFrameTools = EditFrameTools.Select;

            LoadFrameImage();

            LoadSpritesProperties();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            SignalManager.Get<ColorPaletteCleanupSignal>().Dispatch(TabID, FrameIndex);

            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().RemoveListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<OutputSelectedQuadrantSignal>().RemoveListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().RemoveListener(OnColorPaletteControlSelected);
            #endregion
        }

        private void LoadSpritesProperties()
        {
            for (int i = 0; i < CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles.Length; ++i)
            {
                _spritePropertiesX[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipX;
                _spritePropertiesY[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipY;
                _spritePaletteIndices[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].PaletteIndex;
            }

            byte R;
            byte G;
            byte B;

            _doNotSavePalettes = true;

            // Load palettes
            for (int i = 0; i < 4; ++i)
            {
                int color0 = CharacterModel.Animations[AnimationIndex].Palettes[i].Color0;
                R = (byte)(color0 >> 16);
                G = (byte)(color0 >> 8);
                B = (byte)color0;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 0, TabID);

                int color1 = CharacterModel.Animations[AnimationIndex].Palettes[i].Color1;
                R = (byte)(color1 >> 16);
                G = (byte)(color1 >> 8);
                B = (byte)color1;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 1, TabID);

                int color2 = CharacterModel.Animations[AnimationIndex].Palettes[i].Color2;
                R = (byte)(color2 >> 16);
                G = (byte)(color2 >> 8);
                B = (byte)color2;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 2, TabID);

                int color3 = CharacterModel.Animations[AnimationIndex].Palettes[i].Color3;
                R = (byte)(color3 >> 16);
                G = (byte)(color3 >> 8);
                B = (byte)color3;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 3, TabID);
            }

            _doNotSavePalettes = false;
        }

        private void UpdateDialogInfo()
        {
            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<PatternTableModel>().ToArray()
                .Where(p => (p.Model as PatternTableModel).PatternTableType == PatternTableType.Characters);

            Banks = new FileModelVO[banks.Count()];

            int index = 0;

            foreach (FileModelVO item in banks)
            {
                item.Id = index;

                Banks[index] = item;

                index++;
            }

            ProjectModel project = ModelManager.Get<ProjectModel>();

            switch (project.Header.SpriteSize)
            {
                case SpriteSize.s8x8: ProjectGridSize = "8x8"; break;
                case SpriteSize.s8x16: ProjectGridSize = "8x16"; break;
            }

            LoadBankImage();
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            if (!IsActive)
            {
                return;
            }

            LoadBankImage();
        }

        private void OnColorPaletteControlSelected(Color color, int paletteIndex, int colorPosition, string animationID)
        {
            if (_doNotSavePalettes)
            {
                return;
            }

            if (!IsActive || TabID != animationID)
            {
                return;
            }

            int colorInt = (((color.R & 0xff) << 16) | ((color.G & 0xff) << 8) | (color.B & 0xff));

            int prevColorInt = 0;

            switch (colorPosition)
            {
                case 0:
                    prevColorInt = CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color0;
                    CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color0 = colorInt;
                    break;
                case 1:
                    prevColorInt = CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color1;
                    CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color1 = colorInt;
                    break;
                case 2:
                    prevColorInt = CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color2;
                    CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color2 = colorInt;
                    break;
                case 3:
                    prevColorInt = CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color3;
                    CharacterModel.Animations[AnimationIndex].Palettes[paletteIndex].Color3 = colorInt;
                    break;
            }

            // Convert previous color to type Color
            byte R = (byte)(prevColorInt >> 16);
            byte G = (byte)(prevColorInt >> 8);
            byte B = (byte)prevColorInt;

            Color prevColor = Color.FromRgb(R, G, B);

            Task.Factory.StartNew(() => AdjustPaletteCache(paletteIndex, prevColor, color));

            FileHandler.Save();

            LoadFrameImage();
        }

        private void AdjustPaletteCache(int paletteIndex, Color prevColor, Color color)
        {
            foreach (KeyValuePair<Tuple<int, int>, Dictionary<Color, Color>> entry in CharacterViewModel.GroupedPalettes)
            {
                Tuple<int, int> tuple = entry.Key as Tuple<int, int>;

                if (tuple.Item2 == paletteIndex)
                {
                    foreach (KeyValuePair<Color, Color> entry2 in entry.Value)
                    {
                        if (entry2.Value == prevColor)
                        {
                            entry.Value[entry2.Key] = color;
                        }
                    }
                }
            }
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

                int index = ((int)point.X / 8) + (((int)point.Y / 8) * 8);

                SelectedFrameTile = index;

                if (EditFrameTools == EditFrameTools.Select)
                {
                    RectangleVisibility = Visibility.Visible;

                    FlipX = _spritePropertiesX[SelectedFrameTile];
                    FlipY = _spritePropertiesY[SelectedFrameTile];
                    PaletteIndex = (PaletteIndex)_spritePaletteIndices[SelectedFrameTile];
                }
                else if (EditFrameTools == EditFrameTools.Paint && SelectionRectangleVisibility == Visibility.Visible)
                {
                    UpadteSprite(EditFrameTools.Paint);
                }
                else if (EditFrameTools == EditFrameTools.Erase)
                {
                    UpadteSprite(EditFrameTools.Erase);
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

        private void UpadteSprite(EditFrameTools tool)
        {
            if (tool == EditFrameTools.Paint)
            {
                Point characterPoint = new Point
                {
                    X = RectangleLeft,
                    Y = RectangleTop
                };

                PatternTableModel model = Banks[SelectedBank].Model as PatternTableModel;

                string guid = model.PTTiles[SelectedPatternTableTile].GUID;

                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].Point = characterPoint;
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankID = model.GUID;
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankTileID = guid;
            }
            else if (tool == EditFrameTools.Erase)
            {
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankID = string.Empty;
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankTileID = string.Empty;
            }

            FileHandler.Save();

            LoadFrameImage();
        }

        private void SaveProperty(SpriteProperties property, ValueUnion value)
        {
            bool didChange = false;

            switch (property)
            {
                case SpriteProperties.FlipX:

                    if (_spritePropertiesX[SelectedFrameTile] != value.boolean)
                    {
                        _spritePropertiesX[SelectedFrameTile] = value.boolean;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].FlipX = FlipX;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.FlipY:

                    if (_spritePropertiesY[SelectedFrameTile] != value.boolean)
                    {
                        _spritePropertiesY[SelectedFrameTile] = value.boolean;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].FlipY = FlipY;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.PaletteIndex:

                    if (_spritePaletteIndices[SelectedFrameTile] != value.integer)
                    {
                        _spritePaletteIndices[SelectedFrameTile] = value.integer;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].PaletteIndex = (int)PaletteIndex;

                        didChange = true;
                    }
                    break;
            }

            if (didChange)
            {
                FileHandler.Save();

                LoadFrameImage();
            }
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

        private void LoadFrameImage()
        {
            if (CharacterModel == null)
            {
                return;
            }

            WriteableBitmap frameBitmap = CharacterUtils.CreateImage(CharacterModel, AnimationIndex, FrameIndex);

            if (frameBitmap != null)
            {
                FrameImage = Util.ConvertWriteableBitmapToBitmapImage(frameBitmap);
            }
        }
    }
}
