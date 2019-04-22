using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class PatternTableViewModel : ItemViewModel
    {
        private double _actualWidth;
        private double _actualHeight;
        private Point _croppedPoint;
        private PatternTableType _selectedPatternTableType = PatternTableType.Characters;
        private WriteableBitmap _croppedImage;
        private string _projectGridSize;
        private FileModelVO[] _tileSets;
        private int _selectedTileSet;
        private ImageSource _imgSource;
        private ImageSource _pTImage;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private PatternTableModel _model = null;

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public TileSetSelectionChangedCommand TileSetSelectionChangedCommand { get; } = new TileSetSelectionChangedCommand();
        public MoveTileToPatternTableCommand MoveTileToPatternTableCommand { get; } = new MoveTileToPatternTableCommand();
        #endregion

        #region get/set
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

        public PatternTableType SelectedPatternTableType
        {
            get { return _selectedPatternTableType; }
            set
            {
                if (_selectedPatternTableType != value)
                {
                    _selectedPatternTableType = value;

                    OnPropertyChanged("SelectedPatternTableType");

                    Save();
                }
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

        public ImageSource PTImage
        {
            get
            {
                return _pTImage;
            }
            set
            {
                _pTImage = value;

                OnPropertyChanged("PTImage");
            }
        }

        public ImageSource ImgSource
        {
            get
            {
                return _imgSource;
            }
            set
            {
                _imgSource = value;

                OnPropertyChanged("ImgSource");
            }
        }

        public Point CroppedPoint
        {
            get
            {
                return _croppedPoint;
            }
            set
            {
                _croppedPoint = value;

                OnPropertyChanged("CroppedPoint");
            }
        }

        public double ActualHeight
        {
            get
            {
                return _actualHeight;
            }
            set
            {
                _actualHeight = value;

                OnPropertyChanged("ActualHeight");
            }
        }

        public double ActualWidth
        {
            get
            {
                return _actualWidth;
            }
            set
            {
                _actualWidth = value;

                OnPropertyChanged("ActualWidth");
            }
        }

        public WriteableBitmap CroppedImage
        {
            get
            {
                return _croppedImage;
            }
            set
            {
                _croppedImage = value;

                OnPropertyChanged("CroppedImage");
            }
        }

        public FileModelVO[] TileSets
        {
            get { return _tileSets; }
            set
            {
                _tileSets = value;

                OnPropertyChanged("TileSets");
            }
        }

        public int SelectedTileSet
        {
            get { return _selectedTileSet; }
            set
            {
                _selectedTileSet = value;

                OnPropertyChanged("SelectedTileSet");
            }
        }

        public PatternTableModel Model
        {
            get
            {
                if (_model == null)
                {
                    if (ProjectItem?.FileHandler.FileModel is PatternTableModel model)
                    {
                        Model = model;
                    }
                }

                return _model;
            }
            set
            {
                _model = value;

                OnPropertyChanged("Model");
            }
        }
        #endregion

        public PatternTableViewModel()
        {
            UpdateDialogInfo();

            #region Signals
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ProjectConfigurationSavedSignal>().AddListener(OnProjectConfigurationSaved);
            SignalManager.Get<TileSetSelectionChangedSignal>().AddListener(OnTileSetSelectionChanged);
            SignalManager.Get<PatternTableImageUpdatedSignal>().AddListener(OnPatternTableImageUpdated);
            #endregion
        }

        private void OnPatternTableImageUpdated()
        {
            LoadPatternTableImage();
        }

        private void OnTileSetSelectionChanged(FileModelVO fileModel)
        {
            LoadTileSetImage();
        }

        private void OnProjectConfigurationSaved()
        {
            UpdateDialogInfo();
        }

        private void UpdateDialogInfo()
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            switch (project.Header.SpriteSize)
            {
                case SpriteSize.s8x8: ProjectGridSize = "8x8"; break;
                case SpriteSize.s8x16: ProjectGridSize = "8x16"; break;
            }

            TileSets = ProjectFiles.GetModels<TileSetModel>().ToArray();
        }

        public override void OnActivate()
        {
            if (Model != null)
            {
                SelectedPatternTableType = Model.PatternTableType;
            }

            LoadTileSetImage();
            LoadPatternTableImage();
        }

        private void LoadTileSetImage()
        {
            if (TileSets.Length == 0)
            {
                return;
            }

            RectangleVisibility = Visibility.Hidden;
            CroppedImage = null;

            TileSetModel model = TileSets[SelectedTileSet].Model as TileSetModel;

            if (File.Exists(model.ImagePath))
            {
                ImgSource = null;

                ActualWidth = model.ImageWidth;
                ActualHeight = model.ImageHeight;

                BitmapImage bmImage = new BitmapImage();

                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmImage.UriSource = new Uri(model.ImagePath, UriKind.Absolute);
                bmImage.EndInit();
                bmImage.Freeze();

                ImgSource = bmImage;
            }
        }

        private void OnOutputSelectedQuadrant(WriteableBitmap bitmap, Point point)
        {
            CroppedPoint = point;
            CroppedImage = bitmap;

            RectangleVisibility = Visibility.Visible;
            RectangleLeft = point.X;
            RectangleTop = point.Y;
        }

        private void OnMouseWheel(MouseWheelVO vo)
        {
            const double ScaleRate = 1.1;

            if (vo.Delta > 0)
            {
                ActualWidth *= ScaleRate;
                ActualHeight *= ScaleRate;
            }
            else
            {
                ActualWidth /= ScaleRate;
                ActualHeight /= ScaleRate;
            }
        }

        private void Save()
        {
            if (Model != null)
            {
                Model.PatternTableType = SelectedPatternTableType;
                Model.Distribution = PatterTableDistribution.Compact;

                ProjectItem.FileHandler.Save();
            }
        }

        private bool gato = true;

        private void LoadPatternTableImage()
        {
            if (Model == null)
            {
                return;
            }

            PTImage = null;

            WriteableBitmap writeableBmp = BitmapFactory.New(256, 256);
            using (writeableBmp.GetBitmapContext())
            {
                foreach (var tile in Model.PTTiles)
                {
                    if (string.IsNullOrEmpty(tile.GUID))
                    {
                        continue;
                    }

                    // load the image from the tileset and paste it into the pattern table image
                }

                
                writeableBmp.DrawLine(1, 2, 30, 140, gato ? Colors.Green : Colors.Red);

                gato = !gato;
            }

            PTImage = Util.ConvertWriteableBitmapToBitmapImage(writeableBmp);
        }
    }
}
