using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class TileSetModel : AFileModel
    {
        private const string _extensionKey = "extensionTileSets";

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

        public string ImagePath { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }
}
