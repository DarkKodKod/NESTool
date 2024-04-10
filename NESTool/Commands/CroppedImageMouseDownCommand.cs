using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NESTool.Commands;

public class CroppedImageMouseDownCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        MouseButtonEventArgs? mouseEvent = parameter as MouseButtonEventArgs;

        if (mouseEvent?.Source is Image image)
        {
            Point p = mouseEvent.GetPosition(image);

            ProcessImage(image, p);
        }
    }

    private void ProcessImage(Image image, Point point)
    {
        WriteableBitmap writeableBmp = BitmapFactory.New((int)Math.Ceiling(image.ActualWidth), (int)Math.Ceiling(image.ActualHeight));

        using (writeableBmp.GetBitmapContext())
        {
            writeableBmp = BitmapFactory.ConvertToPbgra32Format(image.Source as BitmapSource);

            int x = (int)Math.Floor(point.X / 8);
            int y = (int)Math.Floor(point.Y / 8);

            SignalManager.Get<SelectedPixelSignal>().Dispatch(writeableBmp.Clone(), new Point(x, y));
        }
    }
}
