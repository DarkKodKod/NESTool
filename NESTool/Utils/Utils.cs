using NESTool.Enums;
using System.Text.RegularExpressions;
using System.Windows;

namespace NESTool.Utils
{
    public static class Util
    {
        private static readonly Regex _regex = new Regex(@"^[A-Za-z_][a-zA-Z0-9_\-]*$");

        private const string _folderBanksKey = "folderBanks";
        private const string _folderCharactersKey = "folderCharacters";
        private const string _folderMapsKey = "folderMaps";
        private const string _folderTileSetsKey = "folderTileSets";

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
            switch (extension)
            {
                case ".bank": return ProjectItemType.Bank;
                case ".char": return ProjectItemType.Character;
                case ".map": return ProjectItemType.Map;
                case ".tileset": return ProjectItemType.TileSet;
                default: return ProjectItemType.None;
            }
        }

        public static string GetFolderExtension(string folderName)
        {
            var folderBanks = (string)Application.Current.FindResource(_folderBanksKey);
            var folderCharacters = (string)Application.Current.FindResource(_folderCharactersKey);
            var folderMaps = (string)Application.Current.FindResource(_folderMapsKey);
            var folderTileSets = (string)Application.Current.FindResource(_folderTileSetsKey);

            if (folderName == folderBanks) return ".bank";
            if (folderName == folderCharacters) return ".char";
            if (folderName == folderMaps) return ".map";
            if (folderName == folderTileSets) return ".tileset";

            return string.Empty;
        }
    }
}
