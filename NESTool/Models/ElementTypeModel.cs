using NESTool.Enums;
using System.Windows.Media.Imaging;

namespace NESTool.Models;

public class ElementTypeModel
{
    public string Name { get; set; } = string.Empty;
    public BitmapImage? Image { get; set; }
    public string Path { get; set; } = string.Empty;
    public ProjectItemType Type { get; set; }
}
