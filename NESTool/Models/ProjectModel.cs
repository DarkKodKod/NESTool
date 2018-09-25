using NESTool.Architecture.Model;
using NESTool.Enums;
using Nett;

namespace NESTool.Models
{
    public class ProjectModel : IModel
    {
        public SpriteSize SpriteSize
        {
            get { return _spriteSize; }
            set
            {
                if (_spriteSize != value)
                {
                    _spriteSize = value;
                    Save();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Save();
                }
            }
        }

        private string _name;
        private SpriteSize _spriteSize;
        private string _projectPath;

        public void Reset()
        {
            _projectPath = "";

            SpriteSize = SpriteSize.s8x8;
            Name = "";
        }

        public void Copy(ProjectModel copy)
        {
            SpriteSize = copy.SpriteSize;
            Name = copy.Name;
        }

        public void Load(string path)
        {
            _projectPath = path;

            Copy(Toml.ReadFile<ProjectModel>(_projectPath));
        }

        public void Save(string path)
        {
            _projectPath = path;

            Save();
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_projectPath))
            {
                return;
            }

            Toml.WriteFile(this, _projectPath);
        }
    }
}
