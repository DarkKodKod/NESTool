using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Signals;
using Nett;
using System.Windows;

namespace NESTool.Models
{
    public class ProjectModel : ISingletonModel
    {
        public class INESHeader
        {
            public int INesMapper { get; set; }
            public int CHRSize { get; set; }
            public int PRGSize { get; set; }
            public SpriteSize SpriteSize { get; set; }
            public bool Battery { get; set; }
            public FrameTiming FrameTiming { get; set; }
            public MirroringType MirroringType { get; set; }

            public void Reset()
            {
                INesMapper = 0;
                CHRSize = 0;
                PRGSize = 0;
                SpriteSize = SpriteSize.s8x8;
                Battery = false;
                FrameTiming = FrameTiming.NTSC;
                MirroringType = MirroringType.Vertical;
            }
        }

        public class BuildConfig
        {
            public string OutputFilePath { get; set; }
            public string PatternTableSpriteId { get; set; }
            public string PatternTableBackgroundId { get; set; }

			public BuildConfig()
			{
				Reset();
			}

			public void Reset()
            {
                OutputFilePath = "";
                PatternTableSpriteId = "";
                PatternTableBackgroundId = "";
			}
        }

        public int Version { get; set; }
        public string Name { get; set; } = "";
        public INESHeader Header { get; set; } = new INESHeader();
        /// <summary>
        /// Run-length encoding
        /// </summary>
        public bool RLECompression { get; set; } = false;
        public BuildConfig Build { get; set; } = new BuildConfig();

        [TomlIgnore] public string ProjectFilePath { get; set; }
        [TomlIgnore] public string ProjectPath { get; set; }

        private const string _projectVersionKey = "projectVersion";
        private readonly int _projectVersion = 0;

        public ProjectModel()
        {
            _projectVersion = (int)Application.Current.FindResource(_projectVersionKey);
        }

        public void Reset()
        {
            ProjectFilePath = "";
            ProjectPath = "";
            Name = "";
            RLECompression = false;
            Header.Reset();
            Build.Reset(); ;
        }

        public bool IsOpen()
        {
            return !string.IsNullOrEmpty(ProjectFilePath) && !string.IsNullOrEmpty(ProjectPath);
        }

        public void Copy(ProjectModel copy)
        {
            Name = copy.Name;
            Version = copy.Version;
            RLECompression = copy.RLECompression;

            Build.OutputFilePath = copy.Build.OutputFilePath;
            Build.PatternTableSpriteId = copy.Build.PatternTableSpriteId;
            Build.PatternTableBackgroundId = copy.Build.PatternTableBackgroundId;

            Header.INesMapper = copy.Header.INesMapper;
            Header.CHRSize = copy.Header.CHRSize;
            Header.PRGSize = copy.Header.PRGSize;
            Header.SpriteSize = copy.Header.SpriteSize;
            Header.FrameTiming = copy.Header.FrameTiming;
            Header.MirroringType = copy.Header.MirroringType;
            Header.Battery = copy.Header.Battery;
        }

        public void Load(string path, string filePath)
        {
            ProjectPath = path;
            ProjectFilePath = filePath;

            Copy(Toml.ReadFile<ProjectModel>(ProjectFilePath));
        }

        public void Save(string path)
        {
            ProjectFilePath = path;

            Save();
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(ProjectFilePath))
            {
                return;
            }

            Toml.WriteFile(this, ProjectFilePath);

            SignalManager.Get<ProjectConfigurationSavedSignal>().Dispatch();
        }
    }
}
