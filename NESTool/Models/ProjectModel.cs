using NESTool.Architecture.Model;
using Nett;

namespace NESTool.Models
{
    public class ProjectModel : IModel
    {
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
        private string _projectPath;

        public void Reset()
        {
            _projectPath = "";

            Name = "";
        }

        public void Copy(ProjectModel copy)
        {
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
