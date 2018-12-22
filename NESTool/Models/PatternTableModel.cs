using System.Windows;

namespace NESTool.Models
{
    public class PatternTableModel : AFileModel
    {
        private const string _extensionKey = "extensionPatternTables";

        public override string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_fileExtension))
                {
                    _fileExtension = (string)Application.Current.FindResource(_extensionKey);
                }

                return _fileExtension;
            }
        }
    }
}
