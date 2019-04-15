using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
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
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public TileSetSelectionChangedCommand TileSetSelectionChangedCommand { get; } = new TileSetSelectionChangedCommand();
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
        #endregion

        public PatternTableViewModel()
        {
            UpdateDialogInfo();

            #region Signals
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ProjectConfigurationSavedSignal>().AddListener(OnProjectConfigurationSaved);
            SignalManager.Get<TileSetSelectionChangedSignal>().AddListener(OnTileSetSelectionChanged);
            #endregion
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
            PatternTableModel model = GetModel();
            if (model != null)
            {
                SelectedPatternTableType = model.PatternTableType;
            }

            LoadTileSetImage();
        }

        private void LoadTileSetImage()
        {
            if (TileSets.Length == 0)
            {
                return;
            }

            RectangleVisibility = Visibility.Hidden;

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

        public PatternTableModel GetModel()
        {
            if (ProjectItem?.FileHandler.FileModel is PatternTableModel model)
            {
                return model;
            }

            return null;
        }

        private void Save()
        {
            PatternTableModel model = GetModel();

            if (model != null)
            {
                model.PatternTableType = SelectedPatternTableType;

                ProjectItem.FileHandler.Save();
            }
        }
    }
}
