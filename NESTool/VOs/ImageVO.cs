using System.Windows.Media.Imaging;

namespace NESTool.VOs;

public class ImageVO
{
    public WriteableBitmap Image { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
