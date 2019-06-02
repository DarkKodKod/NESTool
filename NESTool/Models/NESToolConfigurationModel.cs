using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.VOs;
using Nett;
using System.IO;
using System.Windows;

namespace NESTool.Models
{
    public class NESToolConfigurationModel : ISingletonModel
    {
        public int Version { get; set; }
        public int MaxRencetProjectsCount { get; set; } = 9;
        public string DefaultProjectPath { get; set; } = "";
        public int WindowSizeX { get; set; } = 940;
        public int WindowSizeY { get; set; } = 618;
        public bool FullScreen { get; set; } = false;
        public string[] RecentProjects { get; set; }

        private const string _configfileNameKey = "configurationFileName";
        private const string _toolVersionKey = "toolVersion";
        private readonly string _configFileName = "";
        private readonly int _toolVersion = 0;

        private bool _loaded = false;

        public NESToolConfigurationModel()
        {
            RecentProjects = new string[MaxRencetProjectsCount];

            for (int i = 0; i < RecentProjects.Length; ++i)
            {
                RecentProjects[i] = "";
            }

            _configFileName = @".\" + (string)Application.Current.FindResource(_configfileNameKey) + Toml.FileExtension;
            _toolVersion = (int)Application.Current.FindResource(_toolVersionKey);
        }

        public void Copy(NESToolConfigurationModel copy)
        {
            Version = copy.Version;
            DefaultProjectPath = copy.DefaultProjectPath;
            RecentProjects = copy.RecentProjects;
            MaxRencetProjectsCount = copy.MaxRencetProjectsCount;
            WindowSizeX = copy.WindowSizeX;
            WindowSizeY = copy.WindowSizeY;
            FullScreen = copy.FullScreen;
        }

        public void Load()
        {
            bool exists = File.Exists(_configFileName);

            if (exists)
            {
                Copy(Toml.ReadFile<NESToolConfigurationModel>(_configFileName));
            }
            else
            {
                Save();
            }

            WindowVO vo = new WindowVO() { SizeX = WindowSizeX, SizeY = WindowSizeY, IsFullScreen = FullScreen };

            SignalManager.Get<LoadConfigSuccessSignal>().Dispatch();
            SignalManager.Get<SetUpWindowPropertiesSignal>().Dispatch(vo);

            _loaded = true;
        }

        public void Save()
        {
            if (!_loaded)
            {
                return;
            }

            Toml.WriteFile(this, _configFileName);
        }

        /// <summary>
        /// This method insert the project path to the RecentProjects array which has a limit,
        /// so it will shift all entries one slot to the right and left the last one out
        /// </summary>
        /// <param name="projectFullPath"></param>
        public void InsertToRecentProjects(string projectFullPath)
        {
            string[] tmpArray = RecentProjects;

            string[] newArray = new string[MaxRencetProjectsCount];
            newArray[0] = projectFullPath;

            int count = 1;

            for (int i = 0; i < tmpArray.Length; ++i)
            {
                if (tmpArray[i] != projectFullPath)
                {
                    newArray[count] = tmpArray[i];
                    count++;

                    if (count >= MaxRencetProjectsCount)
                    {
                        break;
                    }
                }
            }

            RecentProjects = newArray;
        }
    }
}
