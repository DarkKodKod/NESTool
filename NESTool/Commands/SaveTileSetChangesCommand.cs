﻿using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.Commands;

public class SaveTileSetChangesCommand : Command
{
    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        object[] values = (object[])parameter;
        bool changed = (bool)values[0];

        return changed;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
        {
            return;
        }

        object[] values = (object[])parameter;
        string imagePath = (string)values[1];
        WriteableBitmap wp = (WriteableBitmap)values[2];
        Point croppedPoint = (Point)values[3];
        BitmapSource imgSource = (BitmapSource)values[4];

        Merge(wp, imgSource, croppedPoint, imagePath);

        SignalManager.Get<SavedPixelChangesSignal>().Dispatch();
    }

    private void Merge(WriteableBitmap croppedImage, BitmapSource originalImage, Point croppedPoint, string outputPath)
    {
        originalImage = new FormatConvertedBitmap(originalImage, croppedImage.Format, null, 0);

        WriteableBitmap? cImage = new(originalImage);

        Util.CopyBitmapImageToWriteableBitmap(ref cImage, (int)croppedPoint.X, (int)croppedPoint.Y, croppedImage);

        SaveFile(outputPath, cImage);
    }

    private void SaveFile(string filename, WriteableBitmap? image)
    {
        if (filename == string.Empty)
        {
            return;
        }

        using (FileStream stream = new(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            BmpBitmapEncoder encoder = new();

            BitmapFrame frame = BitmapFrame.Create(image);

            encoder.Frames.Add(frame);
            encoder.Save(stream);
        }
    }
}
