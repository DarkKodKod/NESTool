using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using Nett;
using System.IO;
using System.Windows;

namespace NESTool.Models
{
    public class MappersModel : IModel
    {
        public MapperModel[] Mappers { get; set; }

        private const string _mappersfileNameKey = "mappersFileName";
        private const string _versionKey = "mappersVersion";
        private const string _supportedMappersKey = "supportedMappers";
        private readonly string _configFileName = "";
        private readonly int _version = 0;
        private readonly int _supportedMappers = 0;

        public MappersModel()
        {
            _configFileName = @".\" + (string)Application.Current.FindResource(_mappersfileNameKey) + Toml.FileExtension;
            _version = (int)Application.Current.FindResource(_versionKey);
            _supportedMappers = (int)Application.Current.FindResource(_supportedMappersKey);

            Mappers = new MapperModel[_supportedMappers];
        }

        public void Copy(MappersModel copy)
        {
            Mappers = copy.Mappers;
        }

        public void Load()
        {
            bool exists = File.Exists(_configFileName);

            if (exists)
            {
                Copy(Toml.ReadFile<MappersModel>(_configFileName));
            }
            else
            {
                Save();
            }

            SignalManager.Get<LoadMappersSuccessSignal>().Dispatch();
        }

        private void LoadDefaultMappers()
        {
            Mappers[0] = new MapperModel() { Id = 0, Name = "None" };
            Mappers[1] = new MapperModel() { Id = 4, Name = "MMC3" };
        }

        private void Save()
        {
            LoadDefaultMappers();

            Toml.WriteFile(this, _configFileName);
        }
    }
}
