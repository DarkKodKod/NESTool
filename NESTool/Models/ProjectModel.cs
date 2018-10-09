using NESTool.Architecture.Model;
using Nett;

namespace NESTool.Models
{
    public class ProjectModel : IModel
    {
        public class INESHeader
        {
            public int INesMapper { get; set; }
            public int CHRSize { get; set; }
            public int PRGSize { get; set; }

            public void Reset()
            {
                INesMapper = 0;
                CHRSize = 0;
                PRGSize = 0;
            }
        }

        public string Name { get; set; } = "";
        public INESHeader Header { get; set; } = new INESHeader();

        private string _projectPath;

        public void Reset()
        {
            _projectPath = "";

            Name = "";
            Header.Reset();
        }

        public void Copy(ProjectModel copy)
        {
            Name = copy.Name;

            Header.INesMapper = copy.Header.INesMapper;
            Header.CHRSize = copy.Header.CHRSize;
            Header.PRGSize = copy.Header.PRGSize;
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
