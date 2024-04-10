using System.Windows.Media.Imaging;

namespace NESTool.VOs;

public record ImageVO
{
    public WriteableBitmap? Image { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
}
