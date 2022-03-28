using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class CollisionInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }

    public struct CharacterAnimation
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public float Speed { get; set; }
        public FrameModel[] Frames { get; set; }
        public CollisionInfo CollisionInfo { get; set; }
    }

    public class CharacterModel : AFileModel
    {
        public const int AnimationSize = 64;

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

        public CharacterAnimation[] Animations { get; set; } = new CharacterAnimation[AnimationSize];
        public string[] PaletteIDs { get; set; } = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
    }
}
