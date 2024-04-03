using NESTool.Enums;
using System.Windows.Media.Imaging;

namespace NESTool.Models;

public class ElementTypeModel
{
    public string Name { get; set; }
    public BitmapImage Image { get; set; }
    public string Path { get; set; }
    public ProjectItemType Type { get; set; }
}
