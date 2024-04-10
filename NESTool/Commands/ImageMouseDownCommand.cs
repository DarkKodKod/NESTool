using ArchitectureLibrary.Commands;
using NESTool.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NESTool.Commands;

public class ImageMouseDownCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        MouseButtonEventArgs? mouseEvent = parameter as MouseButtonEventArgs;

        if (mouseEvent?.Source is Image image)
        {
            Point p = mouseEvent.GetPosition(image);

            Util.SendSelectedQuadrantSignal(image, p);
        }
    }
}
