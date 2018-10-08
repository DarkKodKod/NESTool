using NESTool.Architecture.Model;
using Nett;
using System.IO;
using System.Windows;

namespace NESTool.Models
{
    public class NESToolConfigurationModel : IModel
    {
        public int MaxRencetProjectsCount
        {
            get { return _maxRencetProjectsCount; }
            set { _maxRencetProjectsCount = value; }
        }

        public string DefaultProjectPath
        {
            get { return _defaultProjectPath; }
            set { _defaultProjectPath = value; }
        }

        public string[] RecentProjects
        {
            get { return _recentProjects; }
            set { _recentProjects = value; }
        }

        private string[] _recentProjects;
        private string _defaultProjectPath = "";
        private int _maxRencetProjectsCount = 9;

        private const string _configfileNameKey = "configurationFileName";
        private readonly string _configFileName = "";

        public NESToolConfigurationModel()
        {
            _recentProjects = new string[MaxRencetProjectsCount];

            for (int i = 0; i < _recentProjects.Length; ++i)
            {
                _recentProjects[i] = "";
            }

            _configFileName = @".\"+ (string)Application.Current.FindResource(_configfileNameKey) + Toml.FileExtension;
        }

        public void Copy(NESToolConfigurationModel copy)
        {
            DefaultProjectPath = copy.DefaultProjectPath;
            RecentProjects = copy.RecentProjects;
            MaxRencetProjectsCount = copy.MaxRencetProjectsCount;
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
        }

        public void Save()
        {
            Toml.WriteFile(this, _configFileName);
        }

        /// <summary>
        /// This method insert the project path to the RecentProjects array which has a limit,
        /// so it will shift all entries one slot to the right and left the last one out
        /// </summary>
        /// <param name="projectFullPath"></param>
        public void InsertToRecentProjects(string projectFullPath)
        {
            var tmpArray = RecentProjects;

            var newArray = new string[MaxRencetProjectsCount];
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
