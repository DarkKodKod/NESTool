using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class TileSetViewModel : ItemViewModel
    {
        private string _imagePath;
        private double _actualWidth;
        private double _actualHeight;
        private Visibility _gridVisibility = Visibility.Visible;
        private WriteableBitmap _croppedImage;
        private Color _color = Color.FromArgb(0, 255, 255, 255);

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public CroppedImageMouseDownCommand CroppedImageMouseDownCommand { get; } = new CroppedImageMouseDownCommand();
        public ColorPaletteSelectCommand ColorPaletteSelectCommand { get; } = new ColorPaletteSelectCommand();
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
            #endregion
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
        }

        private void OnOutputSelectedQuadrant(WriteableBitmap bitmap)
        {
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
                ImagePath = GetModel().ImagePath;
                ActualWidth = GetModel().ImageWidth;
                ActualHeight = GetModel().ImageHeight;
            }
        }
    }
}
