using NESTool.Architecture.Model;
using Nett;
using System.IO;
using System.Windows;

namespace NESTool.Models
{
    public class NESToolConfigurationModel : IModel
    {
        private const int MaxRencetProjectsCount = 9;

        public string DefaultProjectPath { get; set; }

        public string[] RecentProjects
        {
            get { return _recentProjects; }
            set { _recentProjects = value; }
        }

        private string[] _recentProjects = new string[MaxRencetProjectsCount];

        private const string _configfileNameKey = "configurationFileName";
        private readonly string _configFileName = "";

        public NESToolConfigurationModel()
        {
            _configFileName = @".\"+ (string)Application.Current.FindResource(_configfileNameKey) + Toml.FileExtension;
        }

        public void Copy(NESToolConfigurationModel copy)
        {
            DefaultProjectPath = copy.DefaultProjectPath;
            RecentProjects = copy.RecentProjects;
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
