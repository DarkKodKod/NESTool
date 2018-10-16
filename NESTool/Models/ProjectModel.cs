using ArchitectureLibrary.Model;
using NESTool.Enums;
using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class ProjectModel : IModel
    {
        public class INESHeader
        {
            public int INesMapper { get; set; }
            public int CHRSize { get; set; }
            public int PRGSize { get; set; }
            public SpriteSize SpriteSize { get; set; }
            public bool Battery { get; set; }
            public FrameTiming FrameTiming { get; set; }

            public void Reset()
            {
                INesMapper = 0;
                CHRSize = 0;
                PRGSize = 0;
                SpriteSize = SpriteSize.s8x8;
                Battery = false;
                FrameTiming = FrameTiming.NTSC;
            }
        }

        public int Version { get; set; }
        public string Name { get; set; } = "";
        public INESHeader Header { get; set; } = new INESHeader();
        /// <summary>
        /// Run-length encoding
        /// </summary>
        public bool RLECompression { get; set; } = false;

        private string _projectPath;
        private const string _projectVersionKey = "projectVersion";
        private readonly int _projectVersion = 0;

        public ProjectModel()
        {
            _projectVersion = (int)Application.Current.FindResource(_projectVersionKey);
        }

        public void Reset()
        {
            _projectPath = "";

            Name = "";
            RLECompression = false;
            Header.Reset();
        }

        public void Copy(ProjectModel copy)
        {
            Name = copy.Name;
            Version = copy.Version;
            RLECompression = copy.RLECompression;

            Header.INesMapper = copy.Header.INesMapper;
            Header.CHRSize = copy.Header.CHRSize;
            Header.PRGSize = copy.Header.PRGSize;
            Header.SpriteSize = copy.Header.SpriteSize;
            Header.FrameTiming = copy.Header.FrameTiming;
            Header.Battery = copy.Header.Battery;
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

        public void Save()
        {
            if (string.IsNullOrEmpty(_projectPath))
            {
                return;
            }

            Toml.WriteFile(this, _projectPath);
        }
    }
}
