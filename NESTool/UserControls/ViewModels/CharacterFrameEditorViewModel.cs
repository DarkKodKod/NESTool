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
            FlipY
        }

        private FileModelVO[] _banks;
        private int _selectedBank;
        private string _tabId;
        private int _frameIndex;
        private string _projectGridSize;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private Dictionary<string, WriteableBitmap> _frameBitmapCache = new Dictionary<string, WriteableBitmap>();
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
        private readonly bool[] _spritePropertiesX = new bool[64];
        private readonly bool[] _spritePropertiesY = new bool[64];

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

                    SaveProperty(SpriteProperties.FlipX, value);
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

                    SaveProperty(SpriteProperties.FlipY, value);
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
            #endregion
        }

        private void LoadSpritesProperties()
        {
            for (int i = 0; i < CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles.Length; ++i)
            {
                _spritePropertiesX[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipX;
                _spritePropertiesY[i] = CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[i].FlipY;
            }
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
            Point characterPoint = new Point
            {
                X = RectangleLeft,
                Y = RectangleTop
            };

            PatternTableModel model = Banks[SelectedBank].Model as PatternTableModel;

            if (tool == EditFrameTools.Paint)
            {
                Point croppedPoint = model.PTTiles[SelectedPatternTableTile].Point;
                string guid = model.PTTiles[SelectedPatternTableTile].GUID;

                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].GUID = guid;
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].Point = characterPoint;
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].OriginPoint = croppedPoint;
            }
            else if (tool == EditFrameTools.Erase)
            {
                CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].GUID = string.Empty;
            }

            FileHandler.Save();

            LoadFrameImage();
        }

        private void SaveProperty(SpriteProperties property, bool value)
        {
            if (property == SpriteProperties.FlipX)
            {
                if (_spritePropertiesX[SelectedFrameTile] != value)
                {
                    _spritePropertiesX[SelectedFrameTile] = value;

                    CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].FlipX = FlipX;

                    FileHandler.Save();

                    LoadFrameImage();
                }
            }

            if (property == SpriteProperties.FlipY)
            {
                if (_spritePropertiesY[SelectedFrameTile] != value)
                {
                    _spritePropertiesY[SelectedFrameTile] = value;

                    CharacterModel.Animations[AnimationIndex].Frames[FrameIndex].Tiles[SelectedFrameTile].FlipY = FlipY;

                    FileHandler.Save();

                    LoadFrameImage();
                }
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

            WriteableBitmap frameBitmap = CharacterUtils.CreateImage(CharacterModel, AnimationIndex, FrameIndex, ref _frameBitmapCache);

            FrameImage = Util.ConvertWriteableBitmapToBitmapImage(frameBitmap);
        }
    }
}
