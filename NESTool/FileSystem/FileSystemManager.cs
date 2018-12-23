using NESTool.Enums;
using NESTool.Models;
using NESTool.Utils;
using Nett;
using System.IO;

namespace NESTool.FileSystem
{
    public static class FileSystemManager
    {
        public static void CreateFolder(string name, string path)
        {
            string folderPath = Path.Combine(path, name);

            Directory.CreateDirectory(folderPath);

            CreateMetaFile(name, path, new MetaFileModel());
        }

        public static void CreateFile(string name, string path, ProjectItemType type)
        {
            AFileModel model = Util.FileModelFactory(type);

            string filePath = Path.Combine(path, name + model.FileExtension);

            Toml.WriteFile(model, filePath);

            CreateMetaFile(name, path, new MetaFileModel());
        }

        private static void CreateMetaFile(string name, string path, AFileModel model)
        {
            string filePath = Path.Combine(path, name + model.FileExtension);

            Toml.WriteFile(model, filePath);
        }
    }
}
