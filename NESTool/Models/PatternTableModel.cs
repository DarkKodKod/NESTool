using NESTool.Enums;
using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class PatternTableModel : AFileModel
    {
        private const string _extensionKey = "extensionPatternTables";

        [TomlIgnore]
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

        public PatternTableType PatternTableType { get; set; }
    }
}
