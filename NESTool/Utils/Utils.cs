using NESTool.Enums;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace NESTool.Utils
{
    public static class Util
    {
        private static readonly Regex _regex = new Regex(@"^[A-Za-z_][a-zA-Z0-9_\-]*$");

        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";
        private const string _extensionBanksKey = "extensionBanks";
        private const string _extensionCharactersKey = "extensionCharacters";
        private const string _extensionMapsKey = "extensionMaps";
        private const string _extensionTileSetsKey = "extensionTileSets";

        public static bool ValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return true;
            }

            return _regex != null ? _regex.IsMatch(fileName) : false;
        }

        public static ProjectItemType GetItemType(string extension)
        {
            var extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            var extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            var extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            var extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);

            if (extension == extensionBanks) return ProjectItemType.Bank;
            if (extension == extensionCharacters) return ProjectItemType.Character;
            if (extension == extensionMaps) return ProjectItemType.Map;
            if (extension == extensionTileSets) return ProjectItemType.TileSet;
            
            return ProjectItemType.None;
        }

        public static string GetFolderExtension(string folderName)
        {
            var folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            var folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            var folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            var folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);

            var extensionBanks = (string)Application.Current.FindResource(_extensionBanksKey);
            var extensionCharacters = (string)Application.Current.FindResource(_extensionCharactersKey);
            var extensionMaps = (string)Application.Current.FindResource(_extensionMapsKey);
            var extensionTileSets = (string)Application.Current.FindResource(_extensionTileSetsKey);

            if (folderName == folderBanks) return extensionBanks;
            if (folderName == folderCharacters) return extensionCharacters;
            if (folderName == folderMaps) return extensionMaps;
            if (folderName == folderTileSets) return extensionTileSets;

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
    }
}
