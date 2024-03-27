using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using Nett;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.Models
{
    public class TileSetModel : AFileModel
    {
        private const int MaxPseudonyms = 1024;
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
        public string[] TilePseudonyms { get; set; } = Enumerable.Repeat(string.Empty, MaxPseudonyms).ToArray();

        public static Dictionary<string, WriteableBitmap> BitmapCache = new Dictionary<string, WriteableBitmap>();

        public TileSetModel()
        {
            SignalManager.Get<ProjectItemLoadedSignal>().Listener += OnProjectItemLoaded;
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

        internal int GetIndexFromPosition(Point point)
        {
            int lengthWidth = (int)(ImageWidth / 8.0f);
            int yPos = (int)(point.Y / 8.0f);
            int xPos = (int)(point.X / 8.0f);

            return (lengthWidth * yPos) + xPos;
        }

        public static WriteableBitmap LoadBitmap(TileSetModel tileSetModel, bool forceRedraw = false)
        {
            bool exists = BitmapCache.TryGetValue(tileSetModel.GUID, out WriteableBitmap bitmap);

            if (!exists || forceRedraw)
            {
                Util.GenerateBitmapFromTileSet(tileSetModel, out bitmap);

                BitmapCache[tileSetModel.GUID] = bitmap;
            }

            return bitmap;
        }
    }
}
