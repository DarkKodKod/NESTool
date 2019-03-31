using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using System.IO;

namespace NESTool.ViewModels
{
    public class TileSetViewModel : ItemViewModel
    {
        private string _imagePath;
        private int _actualWidth;
        private int _actualHeight;

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

        public int ActualHeight
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

        public int ActualWidth
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
            #endregion
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
