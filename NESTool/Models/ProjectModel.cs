using NESTool.Architecture.Model;
using NESTool.Enums;
using Nett;

namespace NESTool.Models
{
    public class ProjectModel : IModel
    {
        public SpriteSize SpriteSize { get; set; }
        public string Name { get; set; }

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

            Toml.WriteFile(this, _projectPath);
        }
    }
}
