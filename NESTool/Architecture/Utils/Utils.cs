using System.Text.RegularExpressions;

namespace NESTool.Architecture.Utils
{
    public static class Utils
    {
        private static readonly Regex _regex = new Regex(@"^[A-Za-z_][a-zA-Z0-9_\-]*$");

        public static bool ValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return true;
            }

            return _regex != null ? _regex.IsMatch(fileName) : false;
        }
    }
}
