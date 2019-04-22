using NESTool.Enums;
using Nett;
using System.Windows;

namespace NESTool.Models
{
    public struct PTTileModel
    {
        public string GUID { get; set; }
        public Point Point { get; set; }
    }

    public class PatternTableModel : AFileModel
    {
        private const string _extensionKey = "extensionPatternTables";

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

        public PatternTableType PatternTableType { get; set; }
        public PatterTableDistribution Distribution { get; set; }
        public PTTileModel[] PTTiles { get; set; } = new PTTileModel[256];

        public bool IsFull()
        {
            foreach (var tile in PTTiles)
            {
                if (string.IsNullOrEmpty(tile.GUID))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
