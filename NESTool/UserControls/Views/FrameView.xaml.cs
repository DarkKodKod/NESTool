using ArchitectureLibrary.Signals;
using ArchitectureLibrary.Utils;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.Signals;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for FrameView.xaml
    /// </summary>
    public partial class FrameView : UserControl, INotifyPropertyChanged
    {
        private ImageSource _frameImage;
        private string _projectGridSize = "8x8";
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private bool _flipX = false;
        private bool _flipY = false;
        private bool _backBackground = false;
        private EditFrameTools _editFrameTools;

        public bool[] SpritePropertiesX { get; } = new bool[64];
        public bool[] SpritePropertiesY { get; } = new bool[64];
        public PaletteIndex[] SpritePaletteIndices { get; } = new PaletteIndex[64];
        public bool[] SpritePropertiesBack { get; } = new bool[64];

        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        #endregion

        #region get/set
        public ImageSource FrameImage
        {
            get => _frameImage;
            set
            {
                _frameImage = value;

                OnPropertyChanged("FrameImage");
            }
        }

        public int SelectedFrameTile { get; set; } = -1;

        public string ProjectGridSize
        {
            get => _projectGridSize;
            set
            {
                _projectGridSize = value;

                OnPropertyChanged("ProjectGridSize");
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

        public FrameView()
        {
            InitializeComponent();
        }

        public void OnActivate()
        {
            EditFrameTools = EditFrameTools.Select;

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener += OnOutputSelectedQuadrant;
            SignalManager.Get<CharacterPaletteIndexSignal>().Listener += OnCharacterPaletteIndex;
            #endregion
        }

        public void OnDeactivate()
        {
            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener -= OnOutputSelectedQuadrant;
            SignalManager.Get<CharacterPaletteIndexSignal>().Listener -= OnCharacterPaletteIndex;
            #endregion
        }

        private void OnCharacterPaletteIndex(PaletteIndex paletteIndex)
        {
            SaveProperty(SpriteProperties.PaletteIndex, new ValueUnion { integer = (int)paletteIndex });
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (sender.Name != "imgFrame")
            {
                return;
            }

            RectangleLeft = point.X;
            RectangleTop = point.Y;
            SelectedFrameTile = ((int)point.X / 8) + ((int)point.Y / 8 * 8);

            if (EditFrameTools == EditFrameTools.Select)
            {
                RectangleVisibility = Visibility.Visible;

                FlipX = SpritePropertiesX[SelectedFrameTile];
                FlipY = SpritePropertiesY[SelectedFrameTile];
                BackBackground = SpritePropertiesBack[SelectedFrameTile];

                SignalManager.Get<SelectPaletteIndexSignal>().Dispatch(SpritePaletteIndices[SelectedFrameTile]);
            }
            else if (EditFrameTools == EditFrameTools.Paint)
            {
                Point framePoint = new Point
                {
                    X = RectangleLeft,
                    Y = RectangleTop
                };

                SignalManager.Get<PaintTileSignal>().Dispatch(SelectedFrameTile, framePoint);
            }
            else if (EditFrameTools == EditFrameTools.Erase)
            {
                SignalManager.Get<EraseTileSignal>().Dispatch(SelectedFrameTile);
            }
        }

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
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

                        didChange = true;
                    }
                    break;
                case SpriteProperties.FlipY:

                    if (SpritePropertiesY[SelectedFrameTile] != value.boolean)
                    {
                        SpritePropertiesY[SelectedFrameTile] = value.boolean;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.PaletteIndex:

                    if (SpritePaletteIndices[SelectedFrameTile] != (PaletteIndex)value.integer)
                    {
                        SpritePaletteIndices[SelectedFrameTile] = (PaletteIndex)value.integer;

                        didChange = true;
                    }
                    break;
                case SpriteProperties.BackBackground:

                    if (SpritePropertiesBack[SelectedFrameTile] != value.boolean)
                    {
                        SpritePropertiesBack[SelectedFrameTile] = value.boolean;

                        didChange = true;
                    }
                    break;
            }

            if (didChange)
            {
                SignalManager.Get<SavePropertySignal>().Dispatch(SelectedFrameTile, FlipX, FlipY, value.integer, BackBackground);
            }
        }
    }
}
