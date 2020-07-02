using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using Nett;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.Models
{
    public class TileSetModel : AFileModel
    {
        private const string _extensionKey = "extensionTileSets";

        [TomlIgnore]
        public override string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_fileExtension))
                {
                    _fileExtension = (string)Application.Current.FindResource(_extensionKey);
                }

                return _fileExtension;
            }
        }

        public string ImagePath { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public static Dictionary<string, WriteableBitmap> BitmapCache = new Dictionary<string, WriteableBitmap>();

        public TileSetModel()
        {
            SignalManager.Get<ProjectItemLoadedSignal>().AddListener(OnProjectItemLoaded);
        }

        private void OnProjectItemLoaded(string id)
        {
            if (id != GUID)
            {
                return;
            }

            Util.GenerateBitmapFromTileSet(this, out WriteableBitmap bitmap);

            if (!BitmapCache.ContainsKey(GUID))
            {
                BitmapCache.Add(GUID, bitmap);
            }
        }
    }
}
