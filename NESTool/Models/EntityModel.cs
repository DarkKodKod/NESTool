using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class EntityModel : AFileModel
    {
        private const string _extensionKey = "extensionEntities";

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

        public bool FromCharacters { get; set; } = false;
        public string CharacterId { get; set; }
        public string CharacterAnimationId { get; set; }
        public FrameModel[] Frames { get; set; }
    }
}
