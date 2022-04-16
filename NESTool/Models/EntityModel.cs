using NESTool.Enums;
using Nett;
using System.Collections.Generic;
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

        public EntityModel()
        {
            Frame.FixToGrid = true;
            Frame.Tiles = new CharacterTile[64];
        }

        public EntitySource Source { get; set; }
        public int EntityId { get; set; }
        public string CharacterId { get; set; }
        public string CharacterAnimationId { get; set; }
        public FrameModel Frame { get; set; } = new FrameModel();
        public List<string> Properties { get; set; }
    }
}
