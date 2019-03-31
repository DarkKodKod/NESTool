using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System.IO;

namespace NESTool.ViewModels
{
    public class TileSetViewModel : ItemViewModel
    {
        private string _imagePath;
        private double _actualWidth;
        private double _actualHeight;

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        #endregion

        public TileSetModel GetModel()
        {
            if (ProjectItem?.FileHandler.FileModel is TileSetModel model)
            {
                return model;
            }

            return null;
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

        public TileSetViewModel()
        {
            #region Signals
            SignalManager.Get<UpdateTileSetImageSignal>().AddListener(OnUpdateTileSetImage);
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            #endregion
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
