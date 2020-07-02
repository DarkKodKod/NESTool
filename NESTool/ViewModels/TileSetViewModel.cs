using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class TileSetViewModel : ItemViewModel
    {
		private static readonly Color NullColor = Color.FromArgb(0, 255, 255, 255);

        private string _imagePath;
        private ImageSource _imgSource;
        private double _actualWidth;
        private double _actualHeight;
        private Visibility _gridVisibility = Visibility.Visible;
        private WriteableBitmap _croppedImage;
        private Color _color = NullColor;
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
            FillOutFilters();
        }

        private void OnSavedPixelChanges()
        {
            if (!IsActive)
            {
                return;
            }

            PixelsChanged = false;

            UpdateImage(true);
        }

        private void FillOutFilters()
        {
            Filters[0] = "Image";
            Filters[1] = "*.png;*.bmp;*.gif;*.jpg;*.jpeg;*.jpe;*.jfif;*.tif;*.tiff*.tga";
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

            ImgSource = null;

            ImagePath = filePath;

            using (ImportImageCommand command = new ImportImageCommand())
            {
                object[] parameters = new object[2];
                parameters[0] = filePath;
                parameters[1] = ProjectItem;

                command.Execute(parameters);
            }
        }

        private void OnColorPaletteSelect(Color color)
        {
            if (!IsActive)
            {
                return;
            }

            _color = color;
        }

        private void OnSelectedPixel(WriteableBitmap bitmap, Point point)
        {
            if (!IsActive)
            {
                return;
            }

            if (_color == NullColor)
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
            if (!IsActive)
            {
                return;
            }

            GridVisibility = Visibility.Hidden;
        }

        private void OnShowGrid()
        {
            if (!IsActive)
            {
                return;
            }

            GridVisibility = Visibility.Visible;
        }

        private void OnMouseWheel(MouseWheelVO vo)
        {
            if (!IsActive)
            {
                return;
            }

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

            if (GetModel() == null)
            {
                return;
            }

            GridVisibility = MainWindow.ToolBarTileSetShowHideGrid ? Visibility.Visible : Visibility.Hidden;

            #region Signals
            SignalManager.Get<UpdateTileSetImageSignal>().AddListener(OnUpdateTileSetImage);
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<ShowGridSignal>().AddListener(OnShowGrid);
            SignalManager.Get<HideGridSignal>().AddListener(OnHideGrid);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<SelectedPixelSignal>().AddListener(OnSelectedPixel);
            SignalManager.Get<ColorPaletteSelectSignal>().AddListener(OnColorPaletteSelect);
            SignalManager.Get<SavedPixelChangesSignal>().AddListener(OnSavedPixelChanges);
            SignalManager.Get<BrowseFileSuccessSignal>().AddListener(BrowseFileSuccess);
            #endregion

            if (!string.IsNullOrEmpty(GetModel().ImagePath))
            {
                ProjectModel projectModel = ModelManager.Get<ProjectModel>();

                string path = Path.Combine(projectModel.ProjectPath, GetModel().ImagePath);

                ImagePath = path;
            }

            ActualWidth = GetModel().ImageWidth;
            ActualHeight = GetModel().ImageHeight;

            UpdateImage();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<UpdateTileSetImageSignal>().RemoveListener(OnUpdateTileSetImage);
            SignalManager.Get<MouseWheelSignal>().RemoveListener(OnMouseWheel);
            SignalManager.Get<ShowGridSignal>().RemoveListener(OnShowGrid);
            SignalManager.Get<HideGridSignal>().RemoveListener(OnHideGrid);
            SignalManager.Get<OutputSelectedQuadrantSignal>().RemoveListener(OnOutputSelectedQuadrant);
            SignalManager.Get<SelectedPixelSignal>().RemoveListener(OnSelectedPixel);
            SignalManager.Get<ColorPaletteSelectSignal>().RemoveListener(OnColorPaletteSelect);
            SignalManager.Get<SavedPixelChangesSignal>().RemoveListener(OnSavedPixelChanges);
            SignalManager.Get<BrowseFileSuccessSignal>().RemoveListener(BrowseFileSuccess);
            #endregion
        }

        private void OnUpdateTileSetImage()
        {
            if (!IsActive)
            {
                return;
            }

            if (GetModel() == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(GetModel().ImagePath))
            {
                ProjectModel projectModel = ModelManager.Get<ProjectModel>();

                string path = Path.Combine(projectModel.ProjectPath, GetModel().ImagePath);

                ImagePath = path;
            }

            ActualWidth = GetModel().ImageWidth;
            ActualHeight = GetModel().ImageHeight;

            UpdateImage(true);
        }

        private void UpdateImage(bool redraw = false)
        {
            if (GetModel() == null)
            {
                return;
            }

            ImgSource = null;

            if (redraw || !TileSetModel.BitmapCache.TryGetValue(GetModel().GUID, out WriteableBitmap bitmap))
            {
                Util.GenerateBitmapFromTileSet(GetModel(), out bitmap);

                TileSetModel.BitmapCache[GetModel().GUID] = bitmap;
            }

            ImgSource = bitmap;
        }
    }
}
