using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.UserControls.ViewModels
{
    public enum SpriteProperties
    {
        FlipX,
        FlipY,
        PaletteIndex,
        BackBackground
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ValueUnion
    {
        [FieldOffset(0)] public int integer;
        [FieldOffset(4)] public bool boolean;
    }

    public class CharacterFrameEditorViewModel : ViewModel
    {
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
        private int _selectedBankTile;
        private int _selectedFrameTile = -1;
        private CharacterModel _characterModel;
        private FileHandler _fileHandler;
        private ImageSource _frameImage;
        private EditFrameTools _editFrameTools;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private bool _flipX = false;
        private bool _flipY = false;
        private bool _backBackground = false;
        private readonly bool[] _spritePropertiesX = new bool[64];
        private readonly bool[] _spritePropertiesY = new bool[64];
        private readonly PaletteIndex[] _spritePaletteIndices = new PaletteIndex[64];
        private readonly bool[] _spritePropertiesBack = new bool[64];

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

        public bool BackBackground
        {
            get { return _backBackground; }
            set
            {
                if (EditFrameTools != EditFrameTools.Select)
                {
                    return;
                }

                if (_backBackground != value)
                {
                    _backBackground = value;

                    OnPropertyChanged("BackBackground");

                    SaveProperty(SpriteProperties.BackBackground, new ValueUnion { boolean = value } );
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

        public int SelectedBankTile
        {
            get { return _selectedBankTile; }
            set
            {
                _selectedBankTile = value;

                OnPropertyChanged("SelectedBankTile");
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
            SignalManager.Get<UpdateCharacterImageSignal>().AddListener(OnUpdateCharacterImage);
            #endregion

            EditFrameTools = EditFrameTools.Select;

            LoadFrameImage();

            LoadSpritesProperties();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().RemoveListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<OutputSelectedQuadrantSignal>().RemoveListener(OnOutputSelectedQuadrant);
            SignalManager.Get<UpdateCharacterImageSignal>().RemoveListener(OnUpdateCharacterImage);
            #endregion
        }

        private void LoadSpritesProperties()
        {
            for (int i = 0; i < CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles.Length; ++i)
            {
                _spritePropertiesX[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipX;
                _spritePropertiesY[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipY;
                _spritePaletteIndices[i] = (PaletteIndex)CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].PaletteIndex;
                _spritePropertiesBack[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].BackBackground;
            }
        }

        private void UpdateDialogInfo()
        {
            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<BankModel>().ToArray()
                .Where(p => (p.Model as BankModel).BankUseType == BankUseType.Characters);

            Banks = new FileModelVO[banks.Count()];

            int index = 0;

            foreach (FileModelVO item in banks)
            {
                item.Index = index;

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

        private void OnUpdateCharacterImage()
        {
            if (!IsActive)
            {
                return;
            }

            LoadFrameImage();
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
                    SelectTile();
                }
                else if (EditFrameTools == EditFrameTools.Paint && SelectionRectangleVisibility == Visibility.Visible)
                {
                    PaintTile();
                }
                else if (EditFrameTools == EditFrameTools.Erase)
                {
                    EraseTile();
                }
            }
            else if (sender.Name == "imgBank")
            {
                SelectionRectangleVisibility = Visibility.Visible;
                SelectionRectangleLeft = point.X;
                SelectionRectangleTop = point.Y;

                int index = ((int)point.X / 8) + (((int)point.Y / 8) * 16);

                SelectedBankTile = index;
            }
        }

        private void SelectTile()
        {
            RectangleVisibility = Visibility.Visible;

            FlipX = _spritePropertiesX[SelectedFrameTile];
            FlipY = _spritePropertiesY[SelectedFrameTile];
            BackBackground = _spritePropertiesBack[SelectedFrameTile];

            SignalManager.Get<SelectPaletteIndexSignal>().Dispatch(_spritePaletteIndices[SelectedFrameTile]);
        }

        private void PaintTile()
        {
            Point characterPoint = new Point
            {
                X = RectangleLeft,
                Y = RectangleTop
            };

            BankModel model = Banks[SelectedBank].Model as BankModel;

            string guid = model.PTTiles[SelectedBankTile].GUID;

            CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].Point = characterPoint;
            CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankID = model.GUID;
            CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankTileID = guid;

            FileHandler.Save();

            LoadFrameImage();
        }

        private void EraseTile()
        {
            CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankID = string.Empty;
            CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BankTileID = string.Empty;

            FileHandler.Save();

            LoadFrameImage();
        }

        public void SaveProperty(SpriteProperties property, ValueUnion value)
        {
            if (SelectedFrameTile == -1)
            {
                return;
            }

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

                    if (_spritePaletteIndices[SelectedFrameTile] != (PaletteIndex)value.integer)
                    {
                        _spritePaletteIndices[SelectedFrameTile] = (PaletteIndex)value.integer;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].PaletteIndex = value.integer;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.BackBackground:

                    if (_spritePropertiesBack[SelectedFrameTile] != value.boolean)
                    {
                        _spritePropertiesBack[SelectedFrameTile] = value.boolean;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].BackBackground = BackBackground;

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

            if (!(Banks[SelectedBank].Model is BankModel model))
            {
                return;
            }

            WriteableBitmap bankBitmap = BanksUtils.CreateImage(model, ref _bitmapCache);

            BankImage = Util.ConvertWriteableBitmapToBitmapImage(bankBitmap);
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
