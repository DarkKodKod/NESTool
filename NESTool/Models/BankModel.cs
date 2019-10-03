using NESTool.Enums;
using Nett;
using System.Windows;

namespace NESTool.Models
{
    public struct PTTileModel
    {
        public string GUID { get; set; }
        public string TileSetID { get; set; }
        public Point Point { get; set; }
        public int Group { get; set; }
    }

    public class BankModel : AFileModel
    {
        private const string _extensionKey = "extensionBanks";

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

        public BankType BankType { get; set; }
        public PatternTableType PatternTableType { get; set; }
        public BankTileDistribution Distribution { get; set; }
        public PTTileModel[] PTTiles { get; set; } = new PTTileModel[256];

        public bool IsFull()
        {
            foreach (PTTileModel tile in PTTiles)
            {
                if (string.IsNullOrEmpty(tile.GUID))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetTileIndex(string guid)
        {
            int count = 0;
            foreach (PTTileModel tile in PTTiles)
            {
                if (tile.GUID == guid)
                {
                    return count;
                }

                count++;
            }

            return count;
        }

        public PTTileModel GetTileModel(string guid)
        {
            foreach (PTTileModel tile in PTTiles)
            {
                if (tile.GUID == guid)
                {
                    return tile;
                }
            }

            // return empty
            return new PTTileModel();
        }

        public int GetEmptyTileIndex()
        {
            int i = 0;

            foreach (PTTileModel tile in PTTiles)
            {
                if (string.IsNullOrEmpty(tile.GUID))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }
    }
}
