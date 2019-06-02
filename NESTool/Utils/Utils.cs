using NESTool.Enums;
using NESTool.Models;
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
        private static readonly Regex _regex = new Regex(@"^[A-Za-z_][a-zA-Z0-9_\-\x20]*$");

        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";
        private const string _folderPatternTablesKey = "folderPatternTables";
        private const string _extensionBanksKey = "extensionBanks";
        private const string _extensionCharactersKey = "extensionCharacters";
        private const string _extensionMapsKey = "extensionMaps";
        private const string _extensionTileSetsKey = "extensionTileSets";
        private const string _extensionPatternTablesKey = "extensionPatternTables";

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
                case ProjectItemType.PatternTable: return new PatternTableModel();
                default: return null;
            }
        }

        public static ProjectItemType GetItemType(string extension)
        {
            string extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            string extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            string extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            string extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);
            string extensionPatternTable = (string)Application.Current.FindResource(_extensionPatternTablesKey);

            if (extension == extensionBanks) return ProjectItemType.Bank;
            if (extension == extensionCharacters) return ProjectItemType.Character;
            if (extension == extensionMaps) return ProjectItemType.Map;
            if (extension == extensionTileSets) return ProjectItemType.TileSet;
            if (extension == extensionPatternTable) return ProjectItemType.PatternTable;

            return ProjectItemType.None;
        }

        public static string GetExtensionByType(ProjectItemType type)
        {
            string extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            string extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            string extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            string extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);
            string extensionPatternTable = (string)Application.Current.FindResource(_extensionPatternTablesKey);

            switch (type)
            {
                case ProjectItemType.Bank: return extensionBanks;
                case ProjectItemType.Character: return extensionCharacters;
                case ProjectItemType.Map: return extensionMaps;
                case ProjectItemType.TileSet: return extensionTileSets;
                case ProjectItemType.PatternTable: return extensionPatternTable;
                default: return string.Empty;
            }
        }

        public static string GetFolderExtension(string folderName)
        {
            string folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            string folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            string folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            string folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
            string folderPatternTables = (string)Application.Current.FindResource(_folderPatternTablesKey);

            string extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            string extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            string extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            string extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);
            string extensionPatternTables = (string)Application.Current.FindResource(_extensionPatternTablesKey);

            if (folderName == folderBanks) return extensionBanks;
            if (folderName == folderCharacters) return extensionCharacters;
            if (folderName == folderMaps) return extensionMaps;
            if (folderName == folderTileSets) return extensionTileSets;
            if (folderName == folderPatternTables) return extensionPatternTables;

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
    }
}
