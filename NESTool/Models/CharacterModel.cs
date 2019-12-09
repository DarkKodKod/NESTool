using Nett;
using System.Windows;

namespace NESTool.Models
{
    public struct CharacterTile
    {
        public Point Point { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public int PaletteIndex { get; set; }
        public string BankID { get; set; }
        public string BankTileID { get; set; }
        public bool FrontBackground { get; set; }
    }

    public struct Frame
    {
        public CharacterTile[] Tiles { get; set; }
        public bool FixToGrid { get; set; }
    }

    public struct CharacterAnimation
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public float Speed { get; set; }
        public Frame[] Frames { get; set; }
    }

    public class CharacterModel : AFileModel
    {
        private const string _extensionKey = "extensionCharacters";

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

        public CharacterAnimation[] Animations { get; set; } = new CharacterAnimation[64];
        public string[] PaletteIDs { get; set; } = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
    }
}
