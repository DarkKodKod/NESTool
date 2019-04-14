using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class PatternTableViewModel : ItemViewModel
    {
        private double _actualWidth;
        private double _actualHeight;
        private Point _croppedPoint;
        private WriteableBitmap _croppedImage;

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        #endregion

        #region get/set
        public string ProjectGridSize { get; set; } = "8x8";

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
        #endregion

        public PatternTableViewModel()
        {
            #region Signals
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            #endregion
        }

        private void OnOutputSelectedQuadrant(WriteableBitmap bitmap, Point point)
        {
            CroppedPoint = point;
            CroppedImage = bitmap;
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
    }
}
