using NESTool.Enums;
using NESTool.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private const string _metaExtensionKey = "extensionMetaFile";

        private static string _metaExtension = "";

        public static bool ValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return true;
            }

            return _regex != null ? _regex.IsMatch(fileName) : false;
        }

        public static string GetMetaExtension()
        {
            if (string.IsNullOrEmpty(_metaExtension))
            {
                _metaExtension = (string)Application.Current.FindResource(_metaExtensionKey);
            }

            return _metaExtension;
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
    }
}
