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
        private string _projectGridSize = "8x8";
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

        public bool[] SpritePropertiesX { get; } = new bool[64];
        public bool[] SpritePropertiesY { get; } = new bool[64];
        public PaletteIndex[] SpritePaletteIndices { get; } = new PaletteIndex[64];
        public bool[] SpritePropertiesBack { get; } = new bool[64];

        #region Commands
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public SwitchCharacterFrameViewCommand SwitchCharacterFrameViewCommand { get; } = new SwitchCharacterFrameViewCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        #endregion

        #region get/set
        public bool FlipX
        {
            get => _flipX;
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
            get => _flipY;
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
            get => _backBackground;
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

                    SaveProperty(SpriteProperties.BackBackground, new ValueUnion { boolean = value });
                }
            }
        }

        public FileHandler FileHandler
        {
            get => _fileHandler;
            set
            {
                _fileHandler = value;

                OnPropertyChanged("FileHandler");
            }
        }

        public CharacterModel CharacterModel
        {
            get => _characterModel;
            set
            {
                _characterModel = value;

                OnPropertyChanged("CharacterModel");
            }
        }

        public EditFrameTools EditFrameTools
        {
            get => _editFrameTools;
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
            get => _frameImage;
            set
            {
                _frameImage = value;

                OnPropertyChanged("FrameImage");
            }
        }

        public int SelectedFrameTile
        {
            get => _selectedFrameTile;
            set
            {
                _selectedFrameTile = value;

                OnPropertyChanged("SelectedFrameTile");
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

        public int SelectedBank
        {
            get => _selectedBank;
            set
            {
                _selectedBank = value;

                OnPropertyChanged("SelectedBank");
            }
        }

        public string ProjectGridSize
        {
            get => _projectGridSize;
            set
            {
                _projectGridSize = value;

                OnPropertyChanged("ProjectGridSize");
            }
        }

        public string TabID
        {
            get => _tabId;
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
            get => _frameIndex;
            set
            {
                _frameIndex = value;

                OnPropertyChanged("FrameIndex");
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

        public CharacterFrameEditorViewModel()
        {
            UpdateDialogInfo();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            #region Signals
            SignalManager.Get<UpdateCharacterImageSignal>().Listener += OnUpdateCharacterImage;
            #endregion

            EditFrameTools = EditFrameTools.Select;

            LoadFrameImage();

            LoadSpritesProperties();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<UpdateCharacterImageSignal>().Listener -= OnUpdateCharacterImage;
            #endregion
        }

        private void LoadSpritesProperties()
        {
            for (int i = 0; i < CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles.Length; ++i)
            {
                SpritePropertiesX[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipX;
                SpritePropertiesY[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipY;
                SpritePaletteIndices[i] = (PaletteIndex)CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].PaletteIndex;
                SpritePropertiesBack[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].BackBackground;
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
        }

        private void OnUpdateCharacterImage()
        {
            if (!IsActive)
            {
                return;
            }

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

                    if (SpritePropertiesX[SelectedFrameTile] != value.boolean)
                    {
                        SpritePropertiesX[SelectedFrameTile] = value.boolean;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].FlipX = FlipX;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.FlipY:

                    if (SpritePropertiesY[SelectedFrameTile] != value.boolean)
                    {
                        SpritePropertiesY[SelectedFrameTile] = value.boolean;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].FlipY = FlipY;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.PaletteIndex:

                    if (SpritePaletteIndices[SelectedFrameTile] != (PaletteIndex)value.integer)
                    {
                        SpritePaletteIndices[SelectedFrameTile] = (PaletteIndex)value.integer;

                        CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].PaletteIndex = value.integer;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.BackBackground:

                    if (SpritePropertiesBack[SelectedFrameTile] != value.boolean)
                    {
                        SpritePropertiesBack[SelectedFrameTile] = value.boolean;

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

        public void LoadFrameImage()
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
