using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class TileSetViewModel : ItemViewModel
    {
        private string _imagePath;
        private ImageSource _imgSource;
        private double _actualWidth;
        private double _actualHeight;
        private Visibility _gridVisibility = Visibility.Visible;
        private WriteableBitmap _croppedImage;
        private Color _color = Color.FromArgb(0, 255, 255, 255);
        private bool _pixelsChanged = false;
        private Point _croppedPoint;

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public CroppedImageMouseDownCommand CroppedImageMouseDownCommand { get; } = new CroppedImageMouseDownCommand();
        public ColorPaletteSelectCommand ColorPaletteSelectCommand { get; } = new ColorPaletteSelectCommand();
        public SaveTileSetChangesCommand SaveTileSetChangesCommand { get; } = new SaveTileSetChangesCommand();
        public BrowseFileCommand BrowseFileCommand { get; } = new BrowseFileCommand();
        #endregion

        public TileSetModel GetModel()
        {
            if (ProjectItem?.FileHandler.FileModel is TileSetModel model)
            {
                return model;
            }

            return null;
        }

        #region get/set
        public string[] Filters { get; } = new string[14];

        public bool NewFile { get; } = false;

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

        public bool PixelsChanged
        {
            get
            {
                return _pixelsChanged;
            }
            set
            {
                _pixelsChanged = value;

                OnPropertyChanged("PixelsChanged");
            }
        }

        public Visibility GridVisibility
        {
            get
            {
                return _gridVisibility;
            }
            set
            {
                _gridVisibility = value;

                OnPropertyChanged("GridVisibility");
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

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;

                OnPropertyChanged("ImagePath");
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
        #endregion

        public TileSetViewModel()
        {
            #region Signals
            SignalManager.Get<UpdateTileSetImageSignal>().AddListener(OnUpdateTileSetImage);
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<ShowGridSignal>().AddListener(OnShowGrid);
            SignalManager.Get<HideGridSignal>().AddListener(OnHideGrid);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<SelectedPixelSignal>().AddListener(OnSelectedPixel);
            SignalManager.Get<ColorPalleteSelectSignal>().AddListener(OnColorPalleteSelect);
            SignalManager.Get<SavedPixelChangesSignal>().AddListener(OnSavedPixelChanges);
            SignalManager.Get<BrowseFileSuccessSignal>().AddListener(BrowseFileSuccess);
            #endregion

            FillOutFilters();
        }

        private void OnSavedPixelChanges()
        {
            PixelsChanged = false;

            UpdateImage();
        }

        private void FillOutFilters()
        {
            Filters[0] = "Image";
            Filters[1] = "*.png;*.bmp;*.gif;*.gif;*.jpg;*.jpeg;*.jpe;*.jfif;*.tif;*.tiff*.tga";
            Filters[2] = "PNG";
            Filters[3] = "*.png";
            Filters[4] = "BMP";
            Filters[5] = "*.bmp";
            Filters[6] = "GIF";
            Filters[7] = "*.gif";
            Filters[8] = "JPEG";
            Filters[9] = "*.jpg;*.jpeg;*.jpe;*.jfif";
            Filters[10] = "TIFF";
            Filters[11] = "*.tif;*.tiff";
            Filters[12] = "TGA";
            Filters[13] = "*.tga";

            OnPropertyChanged("Filters");
        }

        private void BrowseFileSuccess(string filePath, bool newFile)
        {
            if (!IsActive)
            {
                return;
            }

            // Only act when is not a new file, only updates the current one
            if (newFile)
            {
                return;
            }

            ImagePath = filePath;

            using (ImportImageCommand command = new ImportImageCommand())
            {
                object[] parameters = new object[2];
                parameters[0] = filePath;
                parameters[1] = ProjectItem;

                command.Execute(parameters);
            }
        }

        private void OnColorPalleteSelect(Color color)
        {
            _color = color;
        }

        private void OnSelectedPixel(WriteableBitmap bitmap, Point point)
        {
            if (_color == Color.FromArgb(0, 255, 255, 255))
            {
                return;
            }

            bitmap.SetPixel((int)point.X, (int)point.Y, _color);

            CroppedImage = bitmap;

            PixelsChanged = true;
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (!IsActive)
            {
                return;
            }

            CroppedPoint = point;
            CroppedImage = bitmap;
        }

        private void OnHideGrid()
        {
            GridVisibility = Visibility.Hidden;
        }

        private void OnShowGrid()
        {
            GridVisibility = Visibility.Visible;
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

        public override void OnActivate()
        {
            base.OnActivate();

            UpdateImage();
        }

        private void OnUpdateTileSetImage()
        {
            UpdateImage();
        }

        private void UpdateImage()
        {
            if (GetModel() == null)
            {
                return;
            }

            if (File.Exists(GetModel().ImagePath))
            {
                ImgSource = null;

                ImagePath = GetModel().ImagePath;
                ActualWidth = GetModel().ImageWidth;
                ActualHeight = GetModel().ImageHeight;

                BitmapImage bmImage = new BitmapImage();

                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmImage.UriSource = new Uri(GetModel().ImagePath, UriKind.Absolute);
                bmImage.EndInit();
                bmImage.Freeze();

                ImgSource = bmImage;
            }
        }
    }
}
