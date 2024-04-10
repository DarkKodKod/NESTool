using System.Collections.Generic;
using System.Windows;

namespace NESTool.Models;

public class CharacterTile
{
    public static readonly int MaxCharacterTiles = 64;

    public Point Point { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public int PaletteIndex { get; set; }
    public string BankID { get; set; } = string.Empty;
    public string BankTileID { get; set; } = string.Empty;
    public bool BackBackground { get; set; }
}

public class FrameModel
{
    public List<CharacterTile>? Tiles { get; set; } = [];
    public bool FixToGrid { get; set; }
}
