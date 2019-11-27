using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class PaletteModel : AFileModel
    {
        private const string _extensionKey = "extensionPalettes";

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

		public Palette Palette { get; set; }
	}
}
