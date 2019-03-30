using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using System.IO;

namespace NESTool.ViewModels
{
    public class TileSetViewModel : ItemViewModel
    {
        private string _imagePath;

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
            }
        }
    }
}
