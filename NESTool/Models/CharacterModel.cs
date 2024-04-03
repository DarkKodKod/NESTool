using Nett;
using System.Collections.Generic;
using System.Windows;

namespace NESTool.Models;

public class CollisionInfo
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}

public class CharacterAnimation
{
    public string ID { get; set; }
    public string Name { get; set; }
    public float Speed { get; set; }
    public List<FrameModel> Frames { get; set; } = [];
    public CollisionInfo CollisionInfo { get; set; }
}

public class CharacterModel : AFileModel
{
    public static readonly int AnimationSize = 64;

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

    public List<CharacterAnimation> Animations { get; set; } = [];
    public string[] PaletteIDs { get; set; } = [string.Empty, string.Empty, string.Empty, string.Empty];
}
