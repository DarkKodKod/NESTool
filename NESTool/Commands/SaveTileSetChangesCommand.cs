using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Commands
{
    public class SaveTileSetChangesCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;
            bool changed = (bool)values[0];

            return changed;
        }

        public override void Execute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            object[] values = (object[])parameter;
            bool changed = (bool)values[0];
            string imagePath = (string)values[1];
            WriteableBitmap wp = (WriteableBitmap)values[2];
            Point croppedPoint = (Point)values[3];

            BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            //FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            //newFormatedBitmapSource.BeginInit();
            //newFormatedBitmapSource.Source = bitmap;
            //newFormatedBitmapSource.DestinationFormat = PixelFormats.Rgb24;
            //newFormatedBitmapSource.EndInit();

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            BitmapImage srcImage = ConvertWriteableBitmapToBitmapImage(wp);

            var cImage = writeableBitmap.Clone();

            Chubs_BitBltMerge(ref cImage, (int)croppedPoint.X, (int)croppedPoint.Y, ref srcImage);

            //SaveFile(imagePath, cImage);
            SaveFile("C:\\Proyectos\\MontezumaNES\\Montezuma\\Images\\gato.bmp", cImage);

            SignalManager.Get<SavedPixelChangesSignal>().Dispatch();
        }

        private void SaveFile(string filename, WriteableBitmap image)
        {
            if (filename == string.Empty)
            {
                return;
            }

            using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();

                BitmapFrame frame = BitmapFrame.Create(image);

                encoder.Frames.Add(frame);
                encoder.Save(stream);
            }
        }

        private BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);

                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        private void Chubs_BitBltMerge(ref WriteableBitmap dest, int nXDest, int nYDest, ref BitmapImage src)
        {
            // copy the source image into a byte buffer
            int src_stride = src.PixelWidth * (src.Format.BitsPerPixel >> 3);
            byte[] src_buffer = new byte[src_stride * src.PixelHeight];
            src.CopyPixels(src_buffer, src_stride, 0);

            // copy the dest image into a byte buffer
//            int dest_stride = src.PixelWidth * (dest.Format.BitsPerPixel >> 3);
//            byte[] dest_buffer = new byte[(src.PixelWidth * src.PixelHeight) << 2];
            byte[] dest_buffer = new byte[(src.PixelWidth * src.PixelHeight) * dest.BackBufferStride];
//            dest.CopyPixels(new Int32Rect(nXDest, nYDest, src.PixelWidth, src.PixelHeight), dest_buffer, dest_stride, 0);

            // do merge (could be made faster through parallelization)
            for (int i = 0; i < src_buffer.Length; i = i + 4)
            {
                //float src_alpha = ((float)src_buffer[i + 3] / 255);
                //dest_buffer[i + 0] = (byte)((src_buffer[i + 0] * src_alpha) + dest_buffer[i + 0] * (1.0 - src_alpha));
                //dest_buffer[i + 1] = (byte)((src_buffer[i + 1] * src_alpha) + dest_buffer[i + 1] * (1.0 - src_alpha));
                //dest_buffer[i + 2] = (byte)((src_buffer[i + 2] * src_alpha) + dest_buffer[i + 2] * (1.0 - src_alpha));
                dest_buffer[i + 0] = 0;
                dest_buffer[i + 1] = 0;
                dest_buffer[i + 2] = 0;
                dest_buffer[i + 3] = 255;
            }

            // copy dest buffer back to the dest WriteableBitmap
            //dest.WritePixels(new Int32Rect(nXDest, nYDest, src.PixelWidth, src.PixelHeight), dest_buffer, dest_stride, 0);
            dest.WritePixels(new Int32Rect(nXDest, nYDest, src.PixelWidth, src.PixelHeight), dest_buffer, dest.BackBufferStride, 0);
        }
    }
}
