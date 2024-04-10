namespace NESTool.VOs;

public record WindowVO
{
    public int SizeX { get; init; }
    public int SizeY { get; init; }
    public bool IsFullScreen { get; init; }
}
