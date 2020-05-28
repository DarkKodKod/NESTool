using ArchitectureLibrary.Signals;
using ColorPalette;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class Util
    {
        public static Color NullColor = GetColorFromInt(0);

        private static readonly Regex _regex = new Regex(@"^[A-Za-z_][a-zA-Z0-9_\-\x20]*$");

        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";
        private const string _folderPalettesKey = "folderPalettes";
        private const string _extensionBanksKey = "extensionBanks";
        private const string _extensionCharactersKey = "extensionCharacters";
        private const string _extensionMapsKey = "extensionMaps";
        private const string _extensionTileSetsKey = "extensionTileSets";
        private const string _extensionPalettesKey = "extensionPalettes";

        public static bool ValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return true;
            }

            return _regex != null ? _regex.IsMatch(fileName) : false;
        }

        public static AFileModel FileModelFactory(ProjectItemType type)
        {
            switch (type)
            {
                case ProjectItemType.Bank: return new BankModel();
                case ProjectItemType.Character: return new CharacterModel();
                case ProjectItemType.Map: return new MapModel();
                case ProjectItemType.TileSet: return new TileSetModel();
                case ProjectItemType.Palette: return new PaletteModel();
                default: return null;
            }
        }

        public static ProjectItemType GetItemType(string extension)
        {
            string extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            string extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            string extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            string extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);
            string extensionPalettes = (string)Application.Current.FindResource(_extensionPalettesKey);

            if (extension == extensionBanks) return ProjectItemType.Bank;
            if (extension == extensionCharacters) return ProjectItemType.Character;
            if (extension == extensionMaps) return ProjectItemType.Map;
            if (extension == extensionTileSets) return ProjectItemType.TileSet;
            if (extension == extensionPalettes) return ProjectItemType.Palette;

            return ProjectItemType.None;
        }

        public static string GetExtensionByType(ProjectItemType type)
        {
            string extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            string extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            string extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            string extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);
            string extensionPalettes = (string)Application.Current.FindResource(_extensionPalettesKey);

            switch (type)
            {
                case ProjectItemType.Bank: return extensionBanks;
                case ProjectItemType.Character: return extensionCharacters;
                case ProjectItemType.Map: return extensionMaps;
                case ProjectItemType.TileSet: return extensionTileSets;
                case ProjectItemType.Palette: return extensionPalettes;
                default: return string.Empty;
            }
        }

        public static string ColorToColorHex(Color color)
        {
            int index = Array.FindIndex(ColorPaletteControl.Colors, element => element == color);
            
            if (index != -1)
            {
                return ColorPaletteControl.HexColors[index];
            }

            return "--";
        }

		public static Color GetColorFromInt(int colorInt)
		{
			byte R = (byte)(colorInt >> 16);
			byte G = (byte)(colorInt >> 8);
			byte B = (byte)colorInt;

			return Color.FromRgb(R, G, B);
		}

        public static string GetFolderExtension(string folderName)
        {
            string folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            string folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            string folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            string folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
            string folderPalettes = (string)Application.Current.FindResource(_folderPalettesKey);

            string extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            string extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            string extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            string extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);
            string extensionPalettes = (string)Application.Current.FindResource(_extensionPalettesKey);

            if (folderName == folderBanks) return extensionBanks;
            if (folderName == folderCharacters) return extensionCharacters;
            if (folderName == folderMaps) return extensionMaps;
            if (folderName == folderTileSets) return extensionTileSets;
            if (folderName == folderPalettes) return extensionPalettes;

            return string.Empty;
        }

        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        public static bool IsPointInTopHalf(ItemsControl itemsControl, DragEventArgs e)
        {
            UIElement selectedItemContainer = GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
            Point relativePosition = e.GetPosition(selectedItemContainer);

            return relativePosition.Y < ((FrameworkElement)selectedItemContainer).ActualHeight / 2;
        }

        public static UIElement GetItemContainerFromPoint(ItemsControl itemsControl, Point p)
        {
            UIElement element = itemsControl.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                {
                    return element;
                }

                object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);

                if (data != DependencyProperty.UnsetValue)
                {
                    return element;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }

            return null;
        }

        public static BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
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

        public static void CopyBitmapImageToWriteableBitmap(ref WriteableBitmap dest, int nXDest, int nYDest, BitmapImage src)
        {
            // copy the source image into a byte buffer
            int src_stride = src.PixelWidth * (src.Format.BitsPerPixel >> 3);
            byte[] src_buffer = new byte[src_stride * src.PixelHeight];
            src.CopyPixels(src_buffer, src_stride, 0);

            int dest_stride = src.PixelWidth * (dest.Format.BitsPerPixel >> 3);
            byte[] dest_buffer = new byte[(src.PixelWidth * src.PixelHeight) << 2];

            // do merge (could be made faster through parallelization), alpha channel is not used at all
            for (int i = 0; i < src_buffer.Length; i += 4)
            {
                dest_buffer[i + 0] = src_buffer[i + 0];
                dest_buffer[i + 1] = src_buffer[i + 1];
                dest_buffer[i + 2] = src_buffer[i + 2];
                dest_buffer[i + 3] = 255;
            }

            // copy dest buffer back to the dest WriteableBitmap
            dest.WritePixels(new Int32Rect(nXDest, nYDest, src.PixelWidth, src.PixelHeight), dest_buffer, dest_stride, 0);
        }

        public static bool SendSelectedQuadrantSignal(Image image, Point point)
        {
            if (image.ActualWidth == 0 || image.ActualHeight == 0)
            {
                return false;
            }

            int imageWidth = (int)Math.Ceiling(image.ActualWidth);
            int imageHeight = (int)Math.Ceiling(image.ActualHeight);

            WriteableBitmap writeableBmp = BitmapFactory.New(imageWidth, imageHeight);

            using (writeableBmp.GetBitmapContext())
            {
                writeableBmp = BitmapFactory.ConvertToPbgra32Format(image.Source as BitmapSource);

                int x = (int)Math.Floor(point.X / 8) * 8;
                int y = (int)Math.Floor(point.Y / 8) * 8;

                WriteableBitmap cropped = writeableBmp.Crop(x, y, 8, 8);

                if (cropped.PixelHeight != 8 || cropped.PixelWidth != 8)
                {
                    return false;
                }

                SignalManager.Get<OutputSelectedQuadrantSignal>().Dispatch(image, cropped, new Point(x, y));
            }

            return true;
        }
    }
}