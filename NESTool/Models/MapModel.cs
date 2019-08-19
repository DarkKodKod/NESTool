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

        public ref MapTile GetTile(int index)
        {
            int attributeTableIndex = index / 4; // TODO: This is not the way to do it!
            int tileIndex = index % 4; // TODO: This is not the way to do it!

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
