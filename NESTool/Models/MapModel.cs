using Nett;
using System.Windows;

namespace NESTool.Models
{
    public struct MapTile
    {
        public Point Point { get; set; }
        public string BankID { get; set; }
        public string BankTileID { get; set; }
    }

    public class AttributeTable
    {
        public MapTile[] MapTile { get; set; }
        public int PaletteIndex { get; set; }
    }

    public class MapModel : AFileModel
    {
        #region Attribute Indices Cache
        private static readonly int[,] IndicesCache = {
            { 0, 1, 32, 33 },
            { 2, 3, 34, 35 }
        };
        #endregion

        private const string _extensionKey = "extensionMaps";

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

        public (int, int) GetAttributeTileIndex(int index)
        {
            for (int i = 0; i < IndicesCache.GetLength(0); ++i)
            {
                for (int j = 0; j < IndicesCache.GetLength(1); ++j)
                {
                    int value = IndicesCache[i, j];

                    if (value == index)
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        public ref MapTile GetTile(int index)
        {
            int attributeTableIndex = GetAttributeTileIndex(index).Item1;
            int tileIndex = GetAttributeTileIndex(index).Item2;

            if (AttributeTable[attributeTableIndex].MapTile == null)
            {
                AttributeTable[attributeTableIndex].MapTile = new MapTile[4];
            }

            return ref AttributeTable[attributeTableIndex].MapTile[tileIndex];
        }

        public AttributeTable[] AttributeTable { get; set; } = new AttributeTable[16 * 15];
        public Palette[] Palettes { get; set; } = new Palette[4];
    }
}
