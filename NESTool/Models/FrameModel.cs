using System.Windows;

namespace NESTool.Models
{
    public struct CharacterTile
    {
        public static readonly int MaxCharacterTiles = 64;

        public Point Point { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public int PaletteIndex { get; set; }
        public string BankID { get; set; }
        public string BankTileID { get; set; }
        public bool BackBackground { get; set; }
    }

    public class FrameModel
    {
        public CharacterTile[] Tiles { get; set; }
        public bool FixToGrid { get; set; }
    }
}
