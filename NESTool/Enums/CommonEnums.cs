namespace NESTool.Enums
{
    public enum SpriteSize
    {
        s8x8 = 0,
        s8x16 = 1
    }

    public enum PatterTableDistribution
    {
        Compact = 0,
        FreeForm = 1
    }

    public enum FrameTiming
    {
        NTSC = 0,
        PAL = 1
    }

    public enum MirroringType
    {
        Vertical = 0,
        Horizontal = 1,
        FourScreens = 2
    }

    public enum ProjectItemType
    {
        None = 0,
        Bank = 1,
        Character = 2,
        Map = 3,
        TileSet = 4,
        PatternTable = 5
    }

    public enum PatternTableType
    {
        Background = 0,
        Characters = 1
    }

    public enum EditFrameTools
    {
        None = 0,
        Select = 1,
        Paint = 2,
        Erase = 3
    }
}
