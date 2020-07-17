namespace NESTool.Enums
{
    public enum SpriteSize
    {
        s8x8    = 0,
        s8x16   = 1
    }

    public enum BankTileDistribution
    {
        Compact     = 0,
        FreeForm    = 1
    }

    public enum FrameTiming
    {
        NTSC    = 0,
        PAL     = 1
    }

    public enum MirroringType
    {
        Vertical    = 0,
        Horizontal  = 1,
        FourScreens = 2
    }

    public enum ProjectItemType
    {
        None        = 0,
        Bank        = 1,
        Character   = 2,
        Map         = 3,
        TileSet     = 4,
        Palette     = 5,
        World       = 6
    }

    public enum BankType
    {
        PatternTable    = 0,
        Size1KB         = 1,
        Size2KB         = 2
    }

    public enum PatternTableType
    {
        Background = 0,
        Characters = 1
    }

    public enum EditFrameTools
    {
        None    = 0,
        Select  = 1,
        Paint   = 2,
        Erase   = 3
    }

    public enum PaletteIndex
    {
        Palette0 = 0,
        Palette1 = 1,
        Palette2 = 2,
        Palette3 = 3
    }

    public enum TileUpdate
    {
        None = 0,
        Normal,
        Erased
    }
}
