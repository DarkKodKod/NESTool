using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NESTool.Commands
{
    public class ImageMouseDownCommand : Command
    {
        public override void Execute(object parameter)
        {
            MouseButtonEventArgs mouseEvent = parameter as MouseButtonEventArgs;

            if (mouseEvent.Source is Image image)
            {
                Point p = mouseEvent.GetPosition(image);

                ProcessImage(image, p);
            }
        }

        private void ProcessImage(Image image, Point point)
        {
            if (image.ActualWidth == 0 || image.ActualHeight == 0)
            {
                return;
            }

            WriteableBitmap writeableBmp = BitmapFactory.New((int)Math.Ceiling(image.ActualWidth), (int)Math.Ceiling(image.ActualHeight));

            using (writeableBmp.GetBitmapContext())
            {
                writeableBmp = BitmapFactory.ConvertToPbgra32Format(image.Source as BitmapSource);

                int x = (int)Math.Floor(point.X / 8) * 8;
                int y = (int)Math.Floor(point.Y / 8) * 8;

                WriteableBitmap cropped = writeableBmp.Crop(x, y, 8, 8);

                if (cropped.PixelHeight != 8 || cropped.PixelWidth != 8)
                {
                    return;
                }

                SignalManager.Get<OutputSelectedQuadrantSignal>().Dispatch(image, cropped, new Point(x, y));
            }
        }
    }
}
